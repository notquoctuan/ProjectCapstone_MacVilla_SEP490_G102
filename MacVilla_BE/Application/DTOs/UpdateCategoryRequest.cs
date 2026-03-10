using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class UpdateCategoryRequest
{
    [Required(ErrorMessage = "Tên danh mục không được để trống.")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên danh mục phải từ 1 đến 255 ký tự.")]
    public string CategoryName { get; set; } = null!;

    [Range(1, long.MaxValue, ErrorMessage = "ID danh mục cha phải lớn hơn 0.")]
    public long? ParentCategoryId { get; set; }
}