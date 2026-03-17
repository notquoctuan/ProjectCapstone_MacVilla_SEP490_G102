using System;

namespace Domain.Entities;

/// <summary>
/// Dòng sản phẩm trong một báo giá (Quotation).
/// Mỗi dòng có giá, chiết khấu riêng và thành tiền được tính tự động.
/// </summary>
public partial class QuotationItem
{
    public long QuotationItemId { get; set; }

    public long? QuotationId { get; set; }

    /// <summary>Liên kết sản phẩm trong catalog (nullable — có thể là sản phẩm ngoài catalog)</summary>
    public long? ProductId { get; set; }

    public string? Sku { get; set; }

    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    /// <summary>Đơn giá trước chiết khấu (VND)</summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>Phần trăm chiết khấu: 0-100</summary>
    public decimal? DiscountPercent { get; set; }

    /// <summary>Thành tiền = Quantity × UnitPrice × (1 - DiscountPercent/100)</summary>
    public decimal? LineTotal { get; set; }

    // Navigation properties
    public virtual Quotation? Quotation { get; set; }
    public virtual Product? Product { get; set; }
}
