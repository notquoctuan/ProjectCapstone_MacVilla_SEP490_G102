using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Quotation
{
    public long QuotationId { get; set; }

    /// <summary>Mã báo giá tự sinh, ví dụ: QT-2024-001</summary>
    public string? QuotationCode { get; set; }

    public long? RfqId { get; set; }

    /// <summary>Người tạo báo giá (Admin/Sale)</summary>
    public long? CreatedBy { get; set; }

    /// <summary>Trạng thái: Draft | SentToCustomer | Approved | Rejected | Expired</summary>
    public string? Status { get; set; }

    /// <summary>Tổng tiền trước chiết khấu và VAT</summary>
    public decimal? SubTotal { get; set; }

    /// <summary>Tổng tiền chiết khấu</summary>
    public decimal? DiscountTotal { get; set; }

    /// <summary>Thuế suất VAT (%): 0, 8, 10</summary>
    public decimal? VatRate { get; set; }

    /// <summary>Tiền thuế VAT</summary>
    public decimal? VatAmount { get; set; }

    /// <summary>Tổng cộng cuối cùng = SubTotal - DiscountTotal + VatAmount</summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>Hạn hiệu lực của báo giá</summary>
    public DateOnly? ValidUntil { get; set; }

    /// <summary>Ghi chú/điểu kiện trong báo giá gửi cho khách</summary>
    public string? Notes { get; set; }

    /// <summary>Ghi chú nội bộ (không gửi khách)</summary>
    public string? InternalNote { get; set; }

    /// <summary>Lý do từ chối (khi Status = Rejected)</summary>
    public string? RejectReason { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Thời điểm gửi báo giá cho khách</summary>
    public DateTime? SentAt { get; set; }

    // Navigation properties
    public virtual Rfq? Rfq { get; set; }
    public virtual User? CreatedByUser { get; set; }
    public virtual ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}
