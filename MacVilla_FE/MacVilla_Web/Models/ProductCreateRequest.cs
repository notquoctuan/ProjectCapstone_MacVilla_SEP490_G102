using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MacVilla_Web.Models
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải từ 0 trở lên")]
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public long CategoryId { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Enable";

        [Display(Name = "Hình ảnh sản phẩm")]
        public List<IFormFile>? ImageFiles { get; set; }
    }
}