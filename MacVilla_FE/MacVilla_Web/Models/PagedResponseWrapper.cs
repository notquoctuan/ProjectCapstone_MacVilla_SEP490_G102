namespace MacVilla_Web.Models;

// Wrapper để giữ tương thích với các file đang dùng MacVilla_Web.Models.PagedResponse<T>.
// (Trong project hiện có một PagedResponse<T> ở global namespace.)
public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

