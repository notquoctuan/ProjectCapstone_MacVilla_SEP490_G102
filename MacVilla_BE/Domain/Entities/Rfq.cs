using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Rfq
{
    public long RfqId { get; set; }

    /// <summary>Mã RFQ tự sinh, ví dụ: RFQ-2024-001</summary>
    public string? RfqCode { get; set; }

    public long? UserId { get; set; }

    // Thông tin khách hàng (có thể khác user đăng nhập, hỗ trợ cả khách vãng lai)
    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    /// <summary>Tên dự án/công trình liên quan</summary>
    public string? ProjectName { get; set; }

    /// <summary>Ngày dự kiến cần giao hàng</summary>
    public DateTime? ExpectedDeliveryDate { get; set; }

    /// <summary>Độ ưu tiên: Low | Normal | High | Urgent</summary>
    public string? Priority { get; set; }

    /// <summary>Mô tả yêu cầu từ khách hàng</summary>
    public string? Description { get; set; }

    /// <summary>Ghi chú nội bộ của Sale/Admin</summary>
    public string? InternalNote { get; set; }

    /// <summary>Trạng thái: Pending | Processing | Quoted | Closed | Cancelled</summary>
    public string? Status { get; set; }

    /// <summary>Sale được phân công xử lý RFQ này</summary>
    public long? AssignedSaleId { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual User? AssignedSale { get; set; }
    public virtual ICollection<RfqItem> RfqItems { get; set; } = new List<RfqItem>();
    public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}
