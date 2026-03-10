// ============================================================
// Application/DTOs/InventoryDtos.cs
// ============================================================
namespace Application.DTOs;

public class InventorySearchRequest
{
    public string? Keyword { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class InventorySummaryResponse
{
    public long InventoryId { get; set; }
    public long? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Sku { get; set; }
    public int? Quantity { get; set; }
    public string? WarehouseLocation { get; set; }
}

public class InventoryDetailResponse : InventorySummaryResponse
{
    public List<InventoryHistoryDto> History { get; set; } = new();
}

public class InventoryHistoryDto
{
    public int? ChangeQty { get; set; }
    public string? Reason { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class UpdateInventoryRequest
{
    public int Quantity { get; set; }
    public string? WarehouseLocation { get; set; }
    public string? Reason { get; set; }
}

public class AdjustInventoryRequest
{
    /// <summary>
    /// Số lượng thay đổi (+ tăng, - giảm).
    /// </summary>
    public int QuantityChange { get; set; }
    public string? Reason { get; set; }
}

public class InventoryStatisticsResponse
{
    public int TotalProducts { get; set; }
    public int TotalQuantity { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
}