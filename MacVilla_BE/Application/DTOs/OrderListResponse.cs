namespace Application.DTOs;

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
