using Domain.Entities;

namespace Domain.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetInventoryByProductIdAsync(long productId);
    Task<Inventory> UpdateInventoryAsync(Inventory inventory);
    Task<bool> ReduceInventoryAsync(long productId, int quantity, string? reason = null);
    Task<bool> RestoreInventoryAsync(long productId, int quantity, string? reason = null);

    Task<Inventory?> GetByIdAsync(long inventoryId);
    Task<(IEnumerable<Inventory> Inventories, int TotalCount)> SearchAsync(string? keyword, int pageNumber, int pageSize);
    Task<List<InventoryHistory>> GetHistoryAsync(long inventoryId);
    Task<Inventory> CreateInventoryAsync(Inventory inventory);
    Task AddInventoryHistoryAsync(long inventoryId, int changeQty, string reason);
}
