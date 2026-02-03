namespace Application.DTOs;

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = null!;
    public long? ParentCategoryId { get; set; }
}
