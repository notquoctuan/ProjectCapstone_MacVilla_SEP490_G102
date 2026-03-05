using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Tên danh mục không được để trống.")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên danh mục phải từ 1 đến 255 ký tự.")]
    public string CategoryName { get; set; } = null!;

    public long? ParentCategoryId { get; set; }
}
