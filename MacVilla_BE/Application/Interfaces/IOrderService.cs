using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<OrderDetailResponse?> GetOrderDetailAsync(long orderId);
    Task<OrderTrackingResponse?> GetOrderTrackingAsync(long orderId);
    Task<PagedResponse<OrderListResponse>> SearchOrdersAsync(OrderSearchRequest request);
    Task<bool> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request);
    Task<bool> CancelOrderAsync(long orderId, string? reason);
}
