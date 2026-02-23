using Domain.Entities;

namespace Domain.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetInventoryByProductIdAsync(long productId);
    Task<Inventory> UpdateInventoryAsync(Inventory inventory);
    Task<bool> ReduceInventoryAsync(long productId, int quantity, string? reason = null);
    Task<bool> RestoreInventoryAsync(long productId, int quantity, string? reason = null);
}
