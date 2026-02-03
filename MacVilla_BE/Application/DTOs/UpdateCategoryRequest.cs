namespace Application.DTOs;

public class UpdateCategoryRequest
{
    public string CategoryName { get; set; } = null!;
    public long? ParentCategoryId { get; set; }
}
