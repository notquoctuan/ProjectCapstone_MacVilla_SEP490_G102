using System;

namespace Domain.Entities;

/// <summary>
/// Dòng sản phẩm trong một yêu cầu báo giá (RFQ).
/// Khách hàng có thể điền tên sản phẩm tự do (ProductId = null) hoặc chọn từ catalog.
/// </summary>
public partial class RfqItem
{
    public long RfqItemId { get; set; }

    public long? RfqId { get; set; }

    /// <summary>Liên kết sản phẩm trong catalog (có thể null nếu khách ghi tên tự do)</summary>
    public long? ProductId { get; set; }

    /// <summary>Mã SKU sản phẩm</summary>
    public string? Sku { get; set; }

    /// <summary>Tên sản phẩm (khách tự nhập hoặc lấy từ catalog)</summary>
    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    /// <summary>Đơn vị tính: Cuộn, Hộp, Mét, Cái, ...</summary>
    public string? Unit { get; set; }

    /// <summary>Ghi chú thêm cho dòng sản phẩm này</summary>
    public string? Note { get; set; }

    // Navigation properties
    public virtual Rfq? Rfq { get; set; }
    public virtual Product? Product { get; set; }
}
