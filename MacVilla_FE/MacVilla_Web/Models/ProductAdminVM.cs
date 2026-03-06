namespace MacVilla_Web.Models
{
    public class ProductAdminVM
    {
        public long ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Name { get; set; }
        public string? CategoryName { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ProductDetailVM
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<ProductImageVM> Images { get; set; } = new();
    }

    public class ProductImageVM
    {
        public long ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }

    public class ProductCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public List<IFormFile>? ImageFiles { get; set; }
    }

    public class ProductUpdateVM
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ImageUrl { get; set; }
    }
}
