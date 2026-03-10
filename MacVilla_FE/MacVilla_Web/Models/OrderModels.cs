namespace MacVilla_Web.Models;

// Wrapper models for Admin Orders page model binding.
// (DTOs also exist in MacVilla_Web.DTOs, but this keeps existing usings compiling.)
public class OrderSearchRequest
{
    public string? Status { get; set; }
    public long? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class OrderListResponse
{
    public long OrderId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public int ItemCount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ShippingStatus { get; set; }
}

