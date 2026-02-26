namespace MacVilla_Web.DTOs;

public class Category
{
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public long? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}

public class PagedResponse<T>
{
    /// <summary>Khớp với BE - BE trả về property "Data"</summary>
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CategorySearchRequest
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
    public long? ParentCategoryId { get; set; }
}

public class UpdateCategoryRequest
{
    public string? CategoryName { get; set; }
    public long? ParentCategoryId { get; set; }
}
