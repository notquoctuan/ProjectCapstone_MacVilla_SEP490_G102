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

    public async Task<bool> ReduceInventoryAsync(long productId, int quantity, string? reason = null)
    {
        var inventory = await GetInventoryByProductIdAsync(productId);
        if (inventory == null)
        {
            return false;
        }

        if (inventory.Quantity < quantity)
        {
            return false; // Insufficient stock
        }

        inventory.Quantity -= quantity;
        await UpdateInventoryAsync(inventory);

        // Track inventory history
        await AddInventoryHistoryAsync(inventory.InventoryId, -quantity, reason ?? "Order processing");

        return true;
    }

    public async Task<bool> RestoreInventoryAsync(long productId, int quantity, string? reason = null)
    {
        var inventory = await GetInventoryByProductIdAsync(productId);
        if (inventory == null)
        {
            return false;
        }

        inventory.Quantity += quantity;
        await UpdateInventoryAsync(inventory);

        // Track inventory history
        await AddInventoryHistoryAsync(inventory.InventoryId, quantity, reason ?? "Order cancellation");

        return true;
    }

    private async Task AddInventoryHistoryAsync(long inventoryId, int changeQty, string reason)
    {
        var history = new InventoryHistory
        {
            InventoryId = inventoryId,
            ChangeQty = changeQty,
            Reason = reason,
            CreatedAt = DateTime.Now
        };

        _context.InventoryHistories.Add(history);
        await _context.SaveChangesAsync();
    }
}
