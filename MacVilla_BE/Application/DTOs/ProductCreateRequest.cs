using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public long CategoryId { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Disable|Enable|Pending)$",
            ErrorMessage = "Trạng thái chỉ được phép là: Disable, Enable hoặc Pending.")]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một ảnh.")]
        [DataType(DataType.Upload)]
        public List<IFormFile> ImageFiles { get; set; } = new();
    }
}