public class Category
{
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
}

public class CategoryVM
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
}

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
}

public class UpdateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
}

public class CategorySearchRequest
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}