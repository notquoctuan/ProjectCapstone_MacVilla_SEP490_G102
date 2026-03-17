using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

// ═══════════════════════════════════════════════════════════════════
// REQUEST DTOs — Admin/Sale creates/updates Quotation
// ═══════════════════════════════════════════════════════════════════

public class CreateQuotationRequest
{
    [Required(ErrorMessage = "RFQ ID không được để trống.")]
    public long RfqId { get; set; }

    [Required(ErrorMessage = "Cần ít nhất 1 dòng báo giá.")]
    [MinLength(1)]
    public List<QuotationItemRequest> Items { get; set; } = new();

    /// <summary>Thuế suất VAT (%): 0, 8, 10</summary>
    [Range(0, 100)]
    public decimal VatRate { get; set; } = 10;

    /// <summary>Số ngày hiệu lực báo giá tính từ ngày tạo</summary>
    [Range(1, 365)]
    public int ValidDays { get; set; } = 30;

    /// <summary>Ghi chú/điều kiện trong báo giá (gửi cho khách)</summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>Ghi chú nội bộ (không gửi khách)</summary>
    [MaxLength(2000)]
    public string? InternalNote { get; set; }
}

public class UpdateQuotationRequest
{
    [Required(ErrorMessage = "Cần ít nhất 1 dòng báo giá.")]
    [MinLength(1)]
    public List<QuotationItemRequest> Items { get; set; } = new();

    [Range(0, 100)]
    public decimal VatRate { get; set; } = 10;

    [Range(1, 365)]
    public int ValidDays { get; set; } = 30;

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [MaxLength(2000)]
    public string? InternalNote { get; set; }
}

public class QuotationItemRequest
{
    public long? ProductId { get; set; }

    public string? Sku { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
    [MaxLength(255)]
    public string ProductName { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public int Quantity { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm.")]
    public decimal UnitPrice { get; set; }

    /// <summary>Phần trăm chiết khấu trên dòng sản phẩm này: 0-100</summary>
    [Range(0, 100)]
    public decimal DiscountPercent { get; set; } = 0;
}

public class RejectQuotationRequest
{
    [MaxLength(1000)]
    public string? Reason { get; set; }
}

// ═══════════════════════════════════════════════════════════════════
// FILTER / SEARCH
// ═══════════════════════════════════════════════════════════════════

public class QuotationFilterRequest
{
    public string? Status { get; set; }
    /// <summary>Tìm theo mã QT, tên khách, công ty</summary>
    public string? Keyword { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ═══════════════════════════════════════════════════════════════════
// RESPONSE DTOs
// ═══════════════════════════════════════════════════════════════════

/// <summary>Dùng cho bảng danh sách báo giá</summary>
public class QuotationListResponse
{
    public long QuotationId { get; set; }
    public string? QuotationCode { get; set; }

    public long? RfqId { get; set; }
    public string? RfqCode { get; set; }

    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? ProjectName { get; set; }

    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateOnly? ValidUntil { get; set; }

    public string? CreatedByName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

/// <summary>Dùng cho trang chi tiết báo giá</summary>
public class QuotationDetailResponse
{
    public long QuotationId { get; set; }
    public string? QuotationCode { get; set; }
    public string? Status { get; set; }

    // Thông tin RFQ & khách hàng
    public long? RfqId { get; set; }
    public string? RfqCode { get; set; }
    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? ProjectName { get; set; }

    // Người tạo báo giá
    public long? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    // Tài chính
    public QuotationFinancialSummary Financial { get; set; } = new();

    public DateOnly? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public string? InternalNote { get; set; }
    public string? RejectReason { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? SentAt { get; set; }

    public List<QuotationItemResponse> Items { get; set; } = new();
}

public class QuotationFinancialSummary
{
    public decimal SubTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class QuotationItemResponse
{
    public long QuotationItemId { get; set; }
    public long? ProductId { get; set; }
    public string? Sku { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? LineTotal { get; set; }
}
