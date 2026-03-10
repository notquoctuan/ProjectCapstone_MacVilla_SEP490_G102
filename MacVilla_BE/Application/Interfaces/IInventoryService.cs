using Application.DTOs;

namespace Application.Interfaces;

public interface IInventoryService
{
    Task<PagedResponse<InventorySummaryResponse>> SearchInventoriesAsync(InventorySearchRequest request);
    Task<InventoryDetailResponse?> GetByInventoryIdAsync(long inventoryId);
    Task<InventoryDetailResponse?> GetByProductIdAsync(long productId);
    Task<IReadOnlyList<InventoryHistoryDto>> GetHistoryAsync(long inventoryId);
    Task<InventoryStatisticsResponse> GetStatisticsAsync();
    Task<InventoryDetailResponse> UpdateInventoryAsync(long productId, UpdateInventoryRequest request);
    Task<InventoryDetailResponse> AdjustInventoryAsync(long productId, AdjustInventoryRequest request);
}