using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class ProductUpdateRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|./?]*$", ErrorMessage = "Tên không được chứa ký tự đặc biệt.")]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public long CategoryId { get; set; }
        public string? Description { get; set; }

        [RegularExpression("^(Disable|Enable|Pending)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = "Pending";

        public List<IFormFile>? NewImageFiles { get; set; }
    }
}