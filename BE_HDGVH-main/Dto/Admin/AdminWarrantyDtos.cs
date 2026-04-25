namespace BE_API.Dto.Admin;

/// <summary>
/// DTO hiển thị danh sách phiếu bảo hành cho Admin
/// </summary>
public class AdminWarrantyTicketListItemDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ClaimCount { get; set; }
    public int PendingClaimCount { get; set; }

    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public int? OrderId { get; set; }
    public string? OrderCode { get; set; }
    public int? ContractId { get; set; }
}

/// <summary>
/// DTO chi tiết phiếu bảo hành cho Admin
/// </summary>
public class AdminWarrantyTicketDetailDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Status { get; set; } = string.Empty;

    public AdminWarrantyCustomerDto Customer { get; set; } = null!;
    public AdminWarrantyOrderDto? Order { get; set; }
    public int? ContractId { get; set; }

    public List<AdminWarrantyClaimDto> Claims { get; set; } = [];
}

public class AdminWarrantyCustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string CustomerType { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
}

public class AdminWarrantyOrderDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public decimal PayableTotal { get; set; }
}

/// <summary>
/// Một dòng trong danh sách yêu cầu bảo hành (claim) — dùng hàng đợi xử lý.
/// </summary>
public class AdminWarrantyClaimListItemDto
{
    public int Id { get; set; }
    public int WarrantyTicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int? OrderId { get; set; }
    public string? OrderCode { get; set; }
    public int VariantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal EstimatedCost { get; set; }
    public string? DefectDescription { get; set; }
}

public class AdminWarrantyClaimDto
{
    public int Id { get; set; }
    public int WarrantyTicketId { get; set; }
    public int VariantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public string? DefectDescription { get; set; }
    public string? ImagesUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Resolution { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO tạo phiếu bảo hành
/// </summary>
public class AdminWarrantyTicketCreateDto
{
    public int CustomerId { get; set; }
    public int? OrderId { get; set; }
    public int? ContractId { get; set; }
    
    /// <summary>
    /// Ngày hết hạn bảo hành. Nếu không nhập, mặc định 12 tháng kể từ ngày tạo.
    /// </summary>
    public DateTime? ValidUntil { get; set; }
}

/// <summary>
/// DTO tạo yêu cầu bảo hành
/// </summary>
public class AdminWarrantyClaimCreateDto
{
    public int VariantId { get; set; }
    public string? DefectDescription { get; set; }
    
    /// <summary>
    /// URL hình ảnh lỗi (nhiều URL cách nhau bằng dấu phẩy hoặc dấu ;)
    /// </summary>
    public string? ImagesUrl { get; set; }
    
    /// <summary>
    /// Chi phí sửa chữa dự kiến (nếu có)
    /// </summary>
    public decimal EstimatedCost { get; set; }
    
    public string? Note { get; set; }
}

/// <summary>
/// DTO cập nhật trạng thái yêu cầu bảo hành
/// </summary>
public class AdminWarrantyClaimUpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Chi phí sửa chữa dự kiến (cập nhật khi cần)
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
    /// <summary>
    /// Kết quả xử lý (khi hoàn thành hoặc từ chối)
    /// </summary>
    public string? Resolution { get; set; }
    
    /// <summary>
    /// Ghi chú thêm
    /// </summary>
    public string? Note { get; set; }
}
