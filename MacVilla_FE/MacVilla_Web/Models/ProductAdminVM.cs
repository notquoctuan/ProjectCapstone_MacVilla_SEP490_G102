namespace MacVilla_Web.Models
{
    public class ProductAdminVM
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public string? CategoryName { get; set; }

        // Code cũ dùng cái này:
        public string? ImageUrl { get; set; }

        // Code mới để hứng dữ liệu từ Backend:
        public List<ProductImageVM> Images { get; set; } = new();

        // Logic bổ trợ: Nếu ImageUrl cũ bị trống, lấy cái ảnh đầu tiên từ danh sách Images
        public string DisplayImage => !string.IsNullOrEmpty(ImageUrl)
            ? ImageUrl
            : (Images.FirstOrDefault(x => x.IsMain)?.ImageUrl ?? "/img/no-image.png");
    }

    public class ProductImageVM
    {
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}