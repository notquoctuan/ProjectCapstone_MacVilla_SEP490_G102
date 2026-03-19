using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Context;

namespace Persistence.Repositories;

// ── Wrapper implements IOrderTransaction ──────────────────────────────
internal sealed class EfOrderTransaction : IOrderTransaction
{
    private readonly IDbContextTransaction _tx;

    public EfOrderTransaction(IDbContextTransaction tx) => _tx = tx;

    public Task CommitAsync() => _tx.CommitAsync();
    public Task RollbackAsync() => _tx.RollbackAsync();
    public ValueTask DisposeAsync() => _tx.DisposeAsync();
}

// ── Repository ────────────────────────────────────────────────────────
public class OrderRepository : IOrderRepository
{
    private readonly MacvilladbContext _context;

    public OrderRepository(MacvilladbContext context)
        => _context = context;

    public async Task<IOrderTransaction> BeginTransactionAsync()
    {
        var tx = await _context.Database.BeginTransactionAsync();
        return new EfOrderTransaction(tx);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        => await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shippings).ThenInclude(s => s.ShippingAddress)
            .Include(o => o.Shippings).ThenInclude(s => s.ShippingMethod)
            .OrderByDescending(o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Order?> GetOrderByIdAsync(long orderId)
        => await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shippings).ThenInclude(s => s.ShippingAddress)
            .Include(o => o.Shippings).ThenInclude(s => s.ShippingMethod)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

    public async Task<Order?> GetOrderDetailByIdAsync(long orderId)
        => await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p!.ProductImages)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p!.Category)
            .Include(o => o.Payments)
            .Include(o => o.Shippings).ThenInclude(s => s.ShippingAddress)
            .Include(o => o.Shippings).ThenInclude(s => s.ShippingMethod)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

    public async Task<Order> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> SearchOrdersAsync(
        string? status, long? userId,
        DateTime? startDate, DateTime? endDate,
        int pageNumber, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);
        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);
        if (startDate.HasValue)
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(o => o.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<bool> OrderExistsAsync(long orderId)
        => await _context.Orders.AnyAsync(o => o.OrderId == orderId);
}