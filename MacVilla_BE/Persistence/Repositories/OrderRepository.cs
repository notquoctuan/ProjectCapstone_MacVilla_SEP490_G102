using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MacvilladbContext _context;

    public OrderRepository(MacvilladbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
                .ThenInclude(s => s.ShippingAddress)
            .Include(o => o.Shippings)
                .ThenInclude(s => s.ShippingMethod)
            .OrderByDescending(o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(long orderId)
    {
        // This method is used for updates, so we need tracking enabled
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
                .ThenInclude(s => s.ShippingAddress)
            .Include(o => o.Shippings)
                .ThenInclude(s => s.ShippingMethod)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Order?> GetOrderDetailByIdAsync(long orderId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.ProductImages)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Category)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
                .ThenInclude(s => s.ShippingAddress)
            .Include(o => o.Shippings)
                .ThenInclude(s => s.ShippingMethod)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> SearchOrdersAsync(
        string? status,
        long? userId,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status != null && o.Status == status);
        }

        // Filter by user
        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        // Filter by date range
        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= endDate.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination and ordering
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<bool> OrderExistsAsync(long orderId)
    {
        return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
    }
}
