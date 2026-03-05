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

    // Valid order statuses (centralised via enum for clarity)
    private static readonly string[] ValidStatuses =
        Enum.GetNames(typeof(OrderStatus));

    public OrderService(IOrderRepository orderRepository, IInventoryRepository inventoryRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<OrderDetailResponse?> GetOrderDetailAsync(long orderId)
    {
        var order = await _orderRepository.GetOrderDetailByIdAsync(orderId);
        if (order == null)
        {
            return null;
        }

        return MapToOrderDetailResponse(order);
    }

    public async Task<OrderTrackingResponse?> GetOrderTrackingAsync(long orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return null;
        }

        return MapToOrderTrackingResponse(order);
    }

    public async Task<PagedResponse<OrderListResponse>> SearchOrdersAsync(OrderSearchRequest request)
    {
        var (orders, totalCount) = await _orderRepository.SearchOrdersAsync(
            request.Status,
            request.UserId,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.PageSize
        );

        var orderList = orders.Select(MapToOrderListResponse).ToList();

        return new PagedResponse<OrderListResponse>
        {
            Data = orderList,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<bool> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request)
    {
        // Validate status
        if (!ValidStatuses.Contains(request.Status))
        {
            throw new ArgumentException($"Invalid status. Valid statuses are: {string.Join(", ", ValidStatuses)}");
        }

        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        var oldStatus = order.Status;
        var newStatus = request.Status;

        // Prevent invalid status transitions
        if (!IsValidStatusTransition(oldStatus, newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from '{oldStatus}' to '{newStatus}'. " +
                $"Valid transitions: {GetValidTransitions(oldStatus)}");
        }

        // Handle inventory based on status change
        await HandleInventoryOnStatusChange(order, oldStatus, newStatus);

        // Update order status
        order.Status = newStatus;
        await _orderRepository.UpdateOrderAsync(order);

        return true;
    }

    public async Task<bool> CancelOrderAsync(long orderId, string? reason)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        // Only allow cancellation if order is not already completed or cancelled
        if (order.Status == "Completed")
        {
            throw new InvalidOperationException("Cannot cancel a completed order.");
        }

        if (order.Status == "Cancelled")
        {
            return true; // Already cancelled
        }

        // Restore inventory if order was processing or shipped
        if (order.Status == "Processing" || order.Status == "Shipped")
        {
            await RestoreOrderInventory(order);
        }

        // Update order status to cancelled
        order.Status = "Cancelled";
        await _orderRepository.UpdateOrderAsync(order);

        return true;
    }

    #region Private Helper Methods

    private static OrderDetailResponse MapToOrderDetailResponse(Order order)
    {
        return new OrderDetailResponse
        {
            OrderId = order.OrderId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
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
    }

    private static OrderListResponse MapToOrderListResponse(Order order)
    {
        return new OrderListResponse
        {
            OrderId = order.OrderId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CustomerName = order.User?.FullName,
            CustomerEmail = order.User?.Email,
            ItemCount = order.OrderItems?.Count ?? 0,
            PaymentStatus = order.Payments?.OrderByDescending(p => p.PaidAt).FirstOrDefault()?.PaymentStatus,
            ShippingStatus = order.Shippings?.OrderByDescending(s => s.DeliveredDate).FirstOrDefault()?.Status
        };
    }

    private static OrderTrackingResponse MapToOrderTrackingResponse(Order order)
    {
        var timeline = new OrderTimelineDto
        {
            IsOrderPlaced = true,
            OrderPlacedAt = order.CreatedAt,
            IsPaymentReceived = order.Payments?.Any(p => p.PaymentStatus == "Paid" || p.PaymentStatus == "Completed") ?? false,
            PaymentReceivedAt = order.Payments?.Where(p => p.PaymentStatus == "Paid" || p.PaymentStatus == "Completed")
                .OrderByDescending(p => p.PaidAt)
                .FirstOrDefault()?.PaidAt,
            IsProcessing = order.Status == "Processing",
            ProcessingStartedAt = order.Status == "Processing" ? order.CreatedAt : null,
            IsShipped = order.Status == "Shipped",
            ShippedAt = order.Shippings?.OrderByDescending(s => s.DeliveredDate).FirstOrDefault()?.DeliveredDate,
            IsDelivered = order.Status == "Completed",
            DeliveredAt = order.Shippings?.Where(s => s.Status == "Delivered")
                .OrderByDescending(s => s.DeliveredDate)
                .FirstOrDefault()?.DeliveredDate,
            IsCancelled = order.Status == "Cancelled"
        };

        // Build status history (simplified - in production, you might want a separate OrderStatusHistory table)
        var statusHistory = new List<OrderStatusHistoryDto>
        {
            new OrderStatusHistoryDto
            {
                Status = order.Status,
                UpdatedAt = order.CreatedAt,
                Notes = "Current status"
            }
        };

        return new OrderTrackingResponse
        {
            OrderId = order.OrderId,
            CurrentStatus = order.Status,
            CreatedAt = order.CreatedAt,
            StatusHistory = statusHistory,
            Timeline = timeline
        };
    }

    private static bool IsValidStatusTransition(string? oldStatus, string newStatus)
    {
        if (string.IsNullOrEmpty(oldStatus))
        {
            return newStatus == "Pending";
        }

        return oldStatus switch
        {
            "Pending" => newStatus == "Processing" || newStatus == "Cancelled",
            "Processing" => newStatus == "Shipped" || newStatus == "Cancelled",
            "Shipped" => newStatus == "Completed" || newStatus == "Cancelled",
            "Completed" => false, // Cannot change from completed
            "Cancelled" => false, // Cannot change from cancelled
            _ => false
        };
    }

    private static string GetValidTransitions(string? status)
    {
        return status switch
        {
            "Pending" => "Processing, Cancelled",
            "Processing" => "Shipped, Cancelled",
            "Shipped" => "Completed, Cancelled",
            "Completed" => "None (final status)",
            "Cancelled" => "None (final status)",
            _ => "Unknown status"
        };
    }

    private async Task HandleInventoryOnStatusChange(Order order, string? oldStatus, string newStatus)
    {
        // When moving to Processing: Reduce inventory
        if (oldStatus != "Processing" && newStatus == "Processing")
        {
            await ReduceOrderInventory(order);
        }
        // When moving from Processing/Shipped to Cancelled: Restore inventory
        else if ((oldStatus == "Processing" || oldStatus == "Shipped") && newStatus == "Cancelled")
        {
            await RestoreOrderInventory(order);
        }
        // When moving from Cancelled back to Processing: Reduce inventory again
        else if (oldStatus == "Cancelled" && newStatus == "Processing")
        {
            await ReduceOrderInventory(order);
        }
    }

    private async Task ReduceOrderInventory(Order order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any())
        {
            return;
        }

        foreach (var item in order.OrderItems)
        {
            if (item.ProductId.HasValue && item.Quantity.HasValue)
            {
                var success = await _inventoryRepository.ReduceInventoryAsync(
                    item.ProductId.Value,
                    item.Quantity.Value,
                    $"Order {order.OrderId} - Status changed to Processing"
                );

                if (!success)
                {
                    throw new InvalidOperationException(
                        $"Insufficient inventory for product ID {item.ProductId}. " +
                        $"Cannot process order {order.OrderId}.");
                }
            }
        }
    }

    private async Task RestoreOrderInventory(Order order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any())
        {
            return;
        }

        foreach (var item in order.OrderItems)
        {
            if (item.ProductId.HasValue && item.Quantity.HasValue)
            {
                await _inventoryRepository.RestoreInventoryAsync(
                    item.ProductId.Value,
                    item.Quantity.Value,
                    $"Order {order.OrderId} - Order cancelled"
                );
            }
        }
    }

    #endregion
}
