using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(long orderId);
    Task<Order?> GetOrderDetailByIdAsync(long orderId);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    Task<(IEnumerable<Order> Orders, int TotalCount)> SearchOrdersAsync(
        string? status, long? userId,
        DateTime? startDate, DateTime? endDate,
        int pageNumber, int pageSize);
    Task<bool> OrderExistsAsync(long orderId);

    // Trả về IAsyncDisposable thay vì IDbContextTransaction
    // → Domain không cần reference EF Core
    Task<IOrderTransaction> BeginTransactionAsync();
}

/// <summary>
/// Wrapper interface cho transaction — Domain không phụ thuộc EF Core
/// </summary>
public interface IOrderTransaction : IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}