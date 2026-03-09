using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải từ 2 đến 200 ký tự.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(1000, 999_999_999, ErrorMessage = "Giá sản phẩm phải từ 1,000 đến 999,999,999 VNĐ.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Range(1, long.MaxValue, ErrorMessage = "Danh mục không hợp lệ.")]
        public long CategoryId { get; set; }

        [StringLength(5000, ErrorMessage = "Mô tả không được vượt quá 5000 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Enable|Disable|Pending)$",
            ErrorMessage = "Trạng thái chỉ được là: Enable, Disable hoặc Pending.")]
        public string Status { get; set; } = "Pending";

        public List<IFormFile>? ImageFiles { get; set; }
    }
}