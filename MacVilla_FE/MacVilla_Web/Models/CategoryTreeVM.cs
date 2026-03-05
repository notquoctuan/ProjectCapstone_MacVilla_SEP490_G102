namespace MacVilla_Web.Models
{
    public class CategoryTreeVM
    {
        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<CategoryTreeVM> Children { get; set; } = new();
    }
}
