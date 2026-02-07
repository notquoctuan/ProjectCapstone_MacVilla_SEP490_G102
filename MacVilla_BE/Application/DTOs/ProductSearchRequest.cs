public class ProductSearchRequest
{
    public string? Name { get; set; }
    public string? CategoryName { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public string? SortOrder { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}