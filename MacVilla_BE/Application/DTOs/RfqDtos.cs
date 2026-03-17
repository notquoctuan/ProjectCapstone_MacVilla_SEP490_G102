using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

// ═══════════════════════════════════════════════════════════════════
// REQUEST DTOs — Customer creates RFQ
// ═══════════════════════════════════════════════════════════════════

public class CreateRfqRequest
{
    [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
    [MaxLength(255)]
    public string CustomerName { get; set; } = null!;

    [MaxLength(255)]
    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [MaxLength(50)]
    public string Phone { get; set; } = null!;

    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(255)]
    public string? ProjectName { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    /// <summary>Low | Normal | High | Urgent</summary>
    public string? Priority { get; set; } = "Normal";

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Cần ít nhất 1 sản phẩm yêu cầu báo giá.")]
    [MinLength(1)]
    public List<RfqItemRequest> Items { get; set; } = new();
}

public class RfqItemRequest
{
    /// <summary>Nullable: khách hàng có thể ghi tên sản phẩm tự do không cần chọn từ catalog</summary>
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

    [MaxLength(500)]
    public string? Note { get; set; }
}

public class UpdateRfqStatusRequest
{
    [Required]
    public string Status { get; set; } = null!;
}

public class AssignSaleRequest
{
    [Required]
    public long AssignedSaleId { get; set; }
}

public class UpdateRfqInternalNoteRequest
{
    [MaxLength(2000)]
    public string? InternalNote { get; set; }
}

// ═══════════════════════════════════════════════════════════════════
// FILTER / SEARCH
// ═══════════════════════════════════════════════════════════════════

public class RfqFilterRequest
{
    public string? Status { get; set; }
    /// <summary>Tìm theo mã RFQ, tên khách, tên công ty, email, điện thoại</summary>
    public string? Keyword { get; set; }
    public long? AssignedSaleId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ═══════════════════════════════════════════════════════════════════
// RESPONSE DTOs
// ═══════════════════════════════════════════════════════════════════

/// <summary>Dùng cho bảng danh sách (admin & customer)</summary>
public class RfqListResponse
{
    public long RfqId { get; set; }
    public string? RfqCode { get; set; }
    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? ProjectName { get; set; }
    public int ItemCount { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? AssignedSaleName { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    /// <summary>Số lượng báo giá đã tạo cho RFQ này</summary>
    public int QuotationCount { get; set; }
}

/// <summary>Dùng cho trang chi tiết (admin & customer)</summary>
public class RfqDetailResponse
{
    public long RfqId { get; set; }
    public string? RfqCode { get; set; }

    // Thông tin khách hàng
    public long? UserId { get; set; }
    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public string? ProjectName { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Priority { get; set; }
    public string? Description { get; set; }
    public string? InternalNote { get; set; }
    public string? Status { get; set; }

    public long? AssignedSaleId { get; set; }
    public string? AssignedSaleName { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<RfqItemResponse> Items { get; set; } = new();
    public List<QuotationSummaryInRfqDto> Quotations { get; set; } = new();
}

public class RfqItemResponse
{
    public long RfqItemId { get; set; }
    public long? ProductId { get; set; }
    public string? Sku { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Note { get; set; }
}

/// <summary>Tóm tắt Quotation hiển thị trong chi tiết RFQ</summary>
public class QuotationSummaryInRfqDto
{
    public long QuotationId { get; set; }
    public string? QuotationCode { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public DateTime? CreatedAt { get; set; }
}
