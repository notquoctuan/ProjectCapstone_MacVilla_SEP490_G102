public class CategoryTreeResponse
{
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<CategoryTreeResponse> Children { get; set; } = new();
}

namespace MacVilla_Web.Models
{
    // Alias/VM để Pages/Index.cshtml.cs sử dụng.
    public class CategoryTreeVM
    {
        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<CategoryTreeVM> Children { get; set; } = new();
    }
}