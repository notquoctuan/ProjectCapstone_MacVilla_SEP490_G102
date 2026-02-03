namespace Application.DTOs;

public class ProductAdminResponse
{
    public long ProductId { get; set; }
    public string? Name { get; set; }
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; } 
    public DateTime? CreatedAt { get; set; }
}