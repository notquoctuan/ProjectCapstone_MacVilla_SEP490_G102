using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    // Các ngưỡng/bộ quy tắc thực tế cho tồn kho
    private const int MaxPageSize = 200;
    private const int MaxKeywordLength = 100;
    private const int MaxTotalQuantityPerProduct = 1_000_000;      // tránh số lượng bất thường
    private const int MaxQuantityChangePerOperation = 100_000;     // một lần điều chỉnh không quá lớn
    private const int MaxReasonLength = 255;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<PagedResponse<InventorySummaryResponse>> SearchInventoriesAsync(InventorySearchRequest request)
    {
        // Chuẩn hóa input tìm kiếm
        var keyword = request.Keyword?.Trim();
        if (!string.IsNullOrEmpty(keyword) && keyword.Length > MaxKeywordLength)
        {
            keyword = keyword.Substring(0, MaxKeywordLength);
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, MaxPageSize);

        var (inventories, totalCount) = await _inventoryRepository.SearchAsync(
            keyword,
            pageNumber,
            pageSize);

        var data = inventories.Select(MapToSummary).ToList();

        return new PagedResponse<InventorySummaryResponse>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<InventoryDetailResponse?> GetByInventoryIdAsync(long inventoryId)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
        if (inventory == null)
        {
            return null;
        }

        var history = await _inventoryRepository.GetHistoryAsync(inventory.InventoryId);
        return MapToDetail(inventory, history);
    }

    public async Task<InventoryDetailResponse?> GetByProductIdAsync(long productId)
    {
        var inventory = await _inventoryRepository.GetInventoryByProductIdAsync(productId);
        if (inventory == null)
        {
            return null;
        }

        var history = await _inventoryRepository.GetHistoryAsync(inventory.InventoryId);
        return MapToDetail(inventory, history);
    }

    public async Task<IReadOnlyList<InventoryHistoryDto>> GetHistoryAsync(long inventoryId)
    {
        var history = await _inventoryRepository.GetHistoryAsync(inventoryId);
        return history
            .Select(h => new InventoryHistoryDto
            {
                ChangeQty = h.ChangeQty,
                Reason = h.Reason,
                CreatedAt = h.CreatedAt
            })
            .ToList();
    }

    public async Task<InventoryStatisticsResponse> GetStatisticsAsync()
    {
        // Lấy toàn bộ tồn kho để tính toán thống kê cơ bản.
        var (inventories, _) = await _inventoryRepository.SearchAsync(
            keyword: null,
            pageNumber: 1,
            pageSize: int.MaxValue);

        var list = inventories.ToList();

        var totalProducts = list.Count;
        var totalQuantity = list.Sum(i => i.Quantity ?? 0);
        var lowStockCount = list.Count(i => (i.Quantity ?? 0) > 0 && (i.Quantity ?? 0) <= 5);
        var outOfStockCount = list.Count(i => (i.Quantity ?? 0) <= 0);

        return new InventoryStatisticsResponse
        {
            TotalProducts = totalProducts,
            TotalQuantity = totalQuantity,
            LowStockCount = lowStockCount,
            OutOfStockCount = outOfStockCount
        };
    }

    public async Task<InventoryDetailResponse> UpdateInventoryAsync(long productId, UpdateInventoryRequest request)
    {
        if (request.Quantity < 0)
        {
            throw new ArgumentException("Số lượng tồn kho không được âm.", nameof(request.Quantity));
        }

        if (request.Quantity > MaxTotalQuantityPerProduct)
        {
            throw new ArgumentException(
                $"Số lượng tồn kho tối đa cho một sản phẩm là {MaxTotalQuantityPerProduct}.",
                nameof(request.Quantity));
        }

        if (!string.IsNullOrWhiteSpace(request.Reason) && request.Reason.Length > MaxReasonLength)
        {
            throw new ArgumentException(
                $"Lý do điều chỉnh không được vượt quá {MaxReasonLength} ký tự.",
                nameof(request.Reason));
        }

        var inventory = await _inventoryRepository.GetInventoryByProductIdAsync(productId);
        if (inventory == null)
        {
            inventory = new Inventory
            {
                ProductId = productId,
                Quantity = request.Quantity,
                WarehouseLocation = request.WarehouseLocation
            };
            inventory = await _inventoryRepository.CreateInventoryAsync(inventory);
        }
        else
        {
            var oldQty = inventory.Quantity ?? 0;
            inventory.Quantity = request.Quantity;
            inventory.WarehouseLocation = request.WarehouseLocation;
            await _inventoryRepository.UpdateInventoryAsync(inventory);

            var diff = request.Quantity - oldQty;
            if (diff != 0)
            {
                await _inventoryRepository.AddInventoryHistoryAsync(
                    inventory.InventoryId,
                    diff,
                    request.Reason ?? "Điều chỉnh tồn kho thủ công");
            }
        }

        var history = await _inventoryRepository.GetHistoryAsync(inventory.InventoryId);
        return MapToDetail(inventory, history);
    }

    public async Task<InventoryDetailResponse> AdjustInventoryAsync(long productId, AdjustInventoryRequest request)
    {
        if (request.QuantityChange == 0)
        {
            throw new ArgumentException("Số lượng điều chỉnh phải khác 0.", nameof(request.QuantityChange));
        }

        if (Math.Abs(request.QuantityChange) > MaxQuantityChangePerOperation)
        {
            throw new ArgumentException(
                $"Mỗi lần điều chỉnh chỉ được phép thay đổi tối đa {MaxQuantityChangePerOperation} đơn vị.",
                nameof(request.QuantityChange));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            // Trong thực tế, các hệ thống lớn thường bắt buộc ghi lý do cho mọi điều chỉnh thủ công.
            throw new ArgumentException("Vui lòng nhập lý do cho việc điều chỉnh tồn kho.", nameof(request.Reason));
        }

        if (request.Reason.Length > MaxReasonLength)
        {
            throw new ArgumentException(
                $"Lý do điều chỉnh không được vượt quá {MaxReasonLength} ký tự.",
                nameof(request.Reason));
        }

        var inventory = await _inventoryRepository.GetInventoryByProductIdAsync(productId);
        if (inventory == null)
        {
            // Nếu chưa có bản ghi tồn kho và yêu cầu là giảm, thì không hợp lệ.
            if (request.QuantityChange < 0)
            {
                throw new InvalidOperationException("Không thể giảm tồn kho cho sản phẩm chưa có tồn kho.");
            }

            inventory = new Inventory
            {
                ProductId = productId,
                Quantity = request.QuantityChange,
            };
            inventory = await _inventoryRepository.CreateInventoryAsync(inventory);
        }
        else
        {
            var newQty = (inventory.Quantity ?? 0) + request.QuantityChange;
            if (newQty < 0)
            {
                throw new InvalidOperationException("Không đủ tồn kho để thực hiện điều chỉnh.");
            }

            if (newQty > MaxTotalQuantityPerProduct)
            {
                throw new InvalidOperationException(
                    $"Số lượng tồn kho vượt quá giới hạn tối đa {MaxTotalQuantityPerProduct} cho một sản phẩm.");
            }

            inventory.Quantity = newQty;
            await _inventoryRepository.UpdateInventoryAsync(inventory);

            await _inventoryRepository.AddInventoryHistoryAsync(
                inventory.InventoryId,
                request.QuantityChange,
                request.Reason ?? (request.QuantityChange > 0 ? "Nhập kho thủ công" : "Xuất kho thủ công"));
        }

        var history = await _inventoryRepository.GetHistoryAsync(inventory.InventoryId);
        return MapToDetail(inventory, history);
    }

    private static InventorySummaryResponse MapToSummary(Inventory inventory)
    {
        return new InventorySummaryResponse
        {
            InventoryId = inventory.InventoryId,
            ProductId = inventory.ProductId,
            ProductName = inventory.Product?.Name,
            Sku = inventory.Sku,
            Quantity = inventory.Quantity,
            WarehouseLocation = inventory.WarehouseLocation
        };
    }

    private static InventoryDetailResponse MapToDetail(Inventory inventory, IEnumerable<InventoryHistory> history)
    {
        var detail = new InventoryDetailResponse
        {
            InventoryId = inventory.InventoryId,
            ProductId = inventory.ProductId,
            ProductName = inventory.Product?.Name,
            Sku = inventory.Sku,
            Quantity = inventory.Quantity,
            WarehouseLocation = inventory.WarehouseLocation,
            History = history.Select(h => new InventoryHistoryDto
            {
                ChangeQty = h.ChangeQty,
                Reason = h.Reason,
                CreatedAt = h.CreatedAt
            }).ToList()
        };

        return detail;
    }
}