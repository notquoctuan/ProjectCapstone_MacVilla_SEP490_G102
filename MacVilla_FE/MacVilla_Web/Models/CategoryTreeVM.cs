public class CategoryTreeResponse
{
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<CategoryTreeResponse> Children { get; set; } = new();
}