

namespace MacVilla_Web.Models
{
    // Danh sách sản phẩm (admin table)
    public class ProductAdminResponse
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

    // Alias/VM dùng cho homepage + nơi khác (giữ tương thích với code cũ)
    public class ProductAdminVM : ProductAdminResponse
    {
    }

    // Chi tiết 1 sản phẩm — BE trả Images là List<ProductImageResponse>
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

    // Ảnh sản phẩm — khớp với BE: class ProductImageResponse
    public class ProductImageResponse
    {
        public long ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }


    // Danh mục dạng cây cho dropdown
    public class CategoryTreeResponse
    {
        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<CategoryTreeResponse> Children { get; set; } = new();
    }
}