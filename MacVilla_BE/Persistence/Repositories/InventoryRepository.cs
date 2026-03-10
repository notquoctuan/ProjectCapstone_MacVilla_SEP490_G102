using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly MacvilladbContext _context;

    public InventoryRepository(MacvilladbContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetInventoryByProductIdAsync(long productId)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<Inventory> UpdateInventoryAsync(Inventory inventory)
    {
        _context.Inventories.Update(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }

    /// <summary>
    /// Giảm tồn kho bằng atomic UPDATE để tránh race condition.
    /// Dùng raw SQL: UPDATE inventory SET quantity = quantity - @qty
    ///               WHERE product_id = @id AND quantity >= @qty
    /// Kiểm tra rowsAffected > 0 để xác nhận thành công.
    /// </summary>
    public async Task<bool> ReduceInventoryAsync(long productId, int quantity, string? reason = null)
    {
        // Atomic UPDATE — tránh read-modify-write race condition
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE inventory SET quantity = quantity - {0} WHERE product_id = {1} AND quantity >= {0}",
            quantity, productId);

        if (rowsAffected == 0)
            return false; // Không đủ hàng hoặc không tồn tại

        // Ghi lịch sử sau khi update thành công
        var inventory = await GetInventoryByProductIdAsync(productId);
        if (inventory != null)
            await AddInventoryHistoryAsync(inventory.InventoryId, -quantity, reason ?? "Order processing");

        return true;
    }

    /// <summary>
    /// Khôi phục tồn kho khi huỷ đơn (cũng atomic để nhất quán).
    /// </summary>
    public async Task<bool> RestoreInventoryAsync(long productId, int quantity, string? reason = null)
    {
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "UPDATE inventory SET quantity = quantity + {0} WHERE product_id = {1}",
            quantity, productId);

        if (rowsAffected == 0)
            return false;

        var inventory = await GetInventoryByProductIdAsync(productId);
        if (inventory != null)
            await AddInventoryHistoryAsync(inventory.InventoryId, quantity, reason ?? "Order cancellation");

        return true;
    }

    public async Task<Inventory?> GetByIdAsync(long inventoryId)
    {
        return await _context.Inventories
            .Include(i => i.InventoryHistories)
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.InventoryId == inventoryId);
    }

    public async Task<(IEnumerable<Inventory> Inventories, int TotalCount)> SearchAsync(string? keyword, int pageNumber, int pageSize)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            query = query.Where(i =>
                (i.Sku != null && i.Sku.ToLower().Contains(normalized)) ||
                (i.Product != null && i.Product.Name != null && i.Product.Name.ToLower().Contains(normalized)));
        }

        var totalCount = await query.CountAsync();

        var inventories = await query
            .OrderBy(i => i.Product!.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (inventories, totalCount);
    }

    public async Task<List<InventoryHistory>> GetHistoryAsync(long inventoryId)
    {
        return await _context.InventoryHistories
            .Where(h => h.InventoryId == inventoryId)
            .OrderByDescending(h => h.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
    {
        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }

    public async Task AddInventoryHistoryAsync(long inventoryId, int changeQty, string reason)
    {
        var history = new InventoryHistory
        {
            InventoryId = inventoryId,
            ChangeQty = changeQty,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryHistories.Add(history);
        await _context.SaveChangesAsync();
    }
}
