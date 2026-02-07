namespace MacVilla_Web.Models
{
    public class ProductAdminVM
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public string? ImageUrl { get; set; } 
        public string? CategoryName { get; set; }
    }
}
