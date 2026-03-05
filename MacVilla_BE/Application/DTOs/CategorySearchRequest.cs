using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CategorySearchRequest
{
    
    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Kích thước trang phải từ 1 đến 100.")]
    public int PageSize { get; set; } = 10;
}
