namespace Application.DTOs
{
    public class ProductDetailResponse
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public long? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<ProductImageResponse> Images { get; set; } = new();
    }

    public class ProductImageResponse
    {
        public long ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}