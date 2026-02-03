namespace Application.DTOs;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
    public int CategoryId { get; set; }
}