namespace MacVilla_Web.Models
{
    public class CategoryVM
    {
        public long CategoryId { get; set; }
        public string Name { get; set; } = null!;

        public long? ParentCategoryId { get; set; }
    }
}