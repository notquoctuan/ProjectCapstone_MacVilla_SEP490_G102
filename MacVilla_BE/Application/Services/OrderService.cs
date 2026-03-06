using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;

    private static readonly string[] ValidStatuses = Enum.GetNames(typeof(OrderStatus));

    public OrderService(
        IOrderRepository orderRepository,
        IInventoryRepository inventoryRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<OrderDetailResponse?> GetOrderDetailAsync(long orderId)
    {
        var order = await _orderRepository.GetOrderDetailByIdAsync(orderId);
        return order == null ? null : MapToOrderDetailResponse(order);
    }

    public async Task<OrderTrackingResponse?> GetOrderTrackingAsync(long orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        return order == null ? null : MapToOrderTrackingResponse(order);
    }

    public async Task<PagedResponse<OrderListResponse>> SearchOrdersAsync(OrderSearchRequest request)
    {
        var (orders, totalCount) = await _orderRepository.SearchOrdersAsync(
            request.Status, request.UserId, request.StartDate, request.EndDate,
            request.PageNumber, request.PageSize);

        return new PagedResponse<OrderListResponse>
        {
            Data = orders.Select(MapToOrderListResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<bool> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request)
    {
        if (!ValidStatuses.Contains(request.Status))
            throw new ArgumentException(
                $"Trạng thái không hợp lệ. Các trạng thái được phép: {string.Join(", ", ValidStatuses)}");

        // Transaction qua IOrderRepository — không cần inject DbContext vào Application layer
        await using var transaction = await _orderRepository.BeginTransactionAsync();
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return false;

            var oldStatus = order.Status;
            var newStatus = request.Status;

            if (!IsValidStatusTransition(oldStatus, newStatus))
                throw new InvalidOperationException(
                    $"Không thể chuyển từ '{oldStatus}' sang '{newStatus}'. " +
                    $"Các chuyển trạng thái hợp lệ: {GetValidTransitions(oldStatus)}");

            await HandleInventoryOnStatusChange(order, oldStatus, newStatus);

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateOrderAsync(order);

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> CancelOrderAsync(long orderId, string? reason)
    {
        await using var transaction = await _orderRepository.BeginTransactionAsync();
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return false;

            if (order.Status == "Completed")
                throw new InvalidOperationException("Không thể huỷ đơn hàng đã hoàn thành.");

            if (order.Status == "Cancelled")
            {
                await transaction.CommitAsync();
                return true; // Đã huỷ rồi
            }

            if (order.Status == "Processing" || order.Status == "Shipped")
                await RestoreOrderInventory(order);

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateOrderAsync(order);

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #region Private Helpers

    private static OrderDetailResponse MapToOrderDetailResponse(Order order) => new()
    {
        OrderId = order.OrderId,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        TotalAmount = order.TotalAmount ?? 0,
        Customer = order.User != null ? new CustomerInfoDto
        {
            UserId = order.User.UserId,
            FullName = order.User.FullName,
            Email = order.User.Email,
            Phone = order.User.Phone
        } : null,
        OrderItems = order.OrderItems.Select(oi => new OrderItemDetailDto
        {
            OrderItemId = oi.OrderItemId,
            ProductId = oi.ProductId ?? 0,
            ProductName = oi.Product?.Name,
            ProductImageUrl = oi.Product?.ProductImages?
                .OrderByDescending(img => img.IsMain)
                .FirstOrDefault()?.ImageUrl,
            CategoryName = oi.Product?.Category?.CategoryName,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice
        }).ToList(),
        Payments = order.Payments.Select(p => new PaymentInfoDto
        {
            PaymentId = p.PaymentId,
            PaymentMethod = p.PaymentMethod,
            PaymentStatus = p.PaymentStatus,
            PaidAt = p.PaidAt
        }).ToList(),
        Shippings = order.Shippings.Select(s => new ShippingInfoDto
        {
            ShippingId = s.ShippingId,
            Status = s.Status,
            ShippingFee = s.ShippingFee,
            DeliveredDate = s.DeliveredDate,
            ShippingAddress = s.ShippingAddress != null ? new ShippingAddressDto
            {
                ReceiverName = s.ShippingAddress.ReceiverName,
                Phone = s.ShippingAddress.Phone,
                AddressLine = s.ShippingAddress.AddressLine,
                Ward = s.ShippingAddress.Ward,
                District = s.ShippingAddress.District,
                City = s.ShippingAddress.City
            } : null,
            ShippingMethod = s.ShippingMethod != null ? new ShippingMethodDto
            {
                MethodName = s.ShippingMethod.MethodName,
                BaseFee = s.ShippingMethod.BaseFee,
                EstimatedDays = s.ShippingMethod.EstimatedDays
            } : null
        }).ToList()
    };

    private static OrderListResponse MapToOrderListResponse(Order order) => new()
    {
        OrderId = order.OrderId,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        TotalAmount = order.TotalAmount ?? 0,
        CustomerName = order.User?.FullName,
        CustomerEmail = order.User?.Email,
        ItemCount = order.OrderItems?.Count ?? 0,
        PaymentStatus = order.Payments?.OrderByDescending(p => p.PaidAt).FirstOrDefault()?.PaymentStatus,
        ShippingStatus = order.Shippings?.OrderByDescending(s => s.DeliveredDate).FirstOrDefault()?.Status
    };

    private static OrderTrackingResponse MapToOrderTrackingResponse(Order order)
    {
        var timeline = new OrderTimelineDto
        {
            IsOrderPlaced = true,
            OrderPlacedAt = order.CreatedAt,
            IsPaymentReceived = order.Payments?.Any(p => p.PaymentStatus == "Paid" || p.PaymentStatus == "Completed") ?? false,
            PaymentReceivedAt = order.Payments?
                .Where(p => p.PaymentStatus == "Paid" || p.PaymentStatus == "Completed")
                .OrderByDescending(p => p.PaidAt).FirstOrDefault()?.PaidAt,
            IsProcessing = order.Status == "Processing",
            ProcessingStartedAt = order.Status == "Processing" ? order.CreatedAt : null,
            IsShipped = order.Status == "Shipped",
            ShippedAt = order.Shippings?.OrderByDescending(s => s.DeliveredDate).FirstOrDefault()?.DeliveredDate,
            IsDelivered = order.Status == "Completed",
            DeliveredAt = order.Shippings?.Where(s => s.Status == "Delivered")
                .OrderByDescending(s => s.DeliveredDate).FirstOrDefault()?.DeliveredDate,
            IsCancelled = order.Status == "Cancelled"
        };

        return new OrderTrackingResponse
        {
            OrderId = order.OrderId,
            CurrentStatus = order.Status,
            CreatedAt = order.CreatedAt,
            StatusHistory = new List<OrderStatusHistoryDto>
            {
                new() { Status = order.Status, UpdatedAt = order.UpdatedAt, Notes = "Trạng thái hiện tại" }
            },
            Timeline = timeline
        };
    }

    private static bool IsValidStatusTransition(string? oldStatus, string newStatus) =>
        oldStatus switch
        {
            null or "" => newStatus == "Pending",
            "Pending" => newStatus is "Processing" or "Cancelled",
            "Processing" => newStatus is "Shipped" or "Cancelled",
            "Shipped" => newStatus is "Completed" or "Cancelled",
            _ => false
        };

    private static string GetValidTransitions(string? status) =>
        status switch
        {
            "Pending" => "Processing, Cancelled",
            "Processing" => "Shipped, Cancelled",
            "Shipped" => "Completed, Cancelled",
            "Completed" => "Không có (trạng thái cuối)",
            "Cancelled" => "Không có (trạng thái cuối)",
            _ => "Trạng thái không xác định"
        };

    private async Task HandleInventoryOnStatusChange(Order order, string? oldStatus, string newStatus)
    {
        if (oldStatus != "Processing" && newStatus == "Processing")
            await ReduceOrderInventory(order);
        else if ((oldStatus == "Processing" || oldStatus == "Shipped") && newStatus == "Cancelled")
            await RestoreOrderInventory(order);
    }

    private async Task ReduceOrderInventory(Order order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any()) return;

        foreach (var item in order.OrderItems)
        {
            if (!item.ProductId.HasValue || !item.Quantity.HasValue) continue;

            var success = await _inventoryRepository.ReduceInventoryAsync(
                item.ProductId.Value, item.Quantity.Value,
                $"Order {order.OrderId} - chuyển sang Processing");

            if (!success)
                throw new InvalidOperationException(
                    $"Không đủ tồn kho cho sản phẩm ID {item.ProductId}. Không thể xử lý đơn {order.OrderId}.");
        }
    }

    private async Task RestoreOrderInventory(Order order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any()) return;

        foreach (var item in order.OrderItems)
        {
            if (!item.ProductId.HasValue || !item.Quantity.HasValue) continue;

            await _inventoryRepository.RestoreInventoryAsync(
                item.ProductId.Value, item.Quantity.Value,
                $"Order {order.OrderId} - đơn bị huỷ");
        }
    }

    #endregion
}