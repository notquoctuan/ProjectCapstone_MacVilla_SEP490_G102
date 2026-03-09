using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    /// <summary>
    /// Cập nhật thông tin sản phẩm.
    /// Quản lý ảnh linh hoạt:
    ///   - NewImageFiles   : upload thêm ảnh mới
    ///   - DeleteImageIds  : xóa ảnh theo ID
    ///   - MainImageId     : đặt ảnh chính (từ ảnh cũ còn lại)
    /// </summary>
    public class ProductUpdateRequest
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

        // ── Quản lý ảnh ───────────────────────────────────────────────

        /// <summary>Upload thêm ảnh mới (không xóa ảnh cũ)</summary>
        public List<IFormFile>? NewImageFiles { get; set; }

        /// <summary>Danh sách ImageId cần xóa</summary>
        public List<long>? DeleteImageIds { get; set; }

        /// <summary>ImageId muốn đặt làm ảnh chính (phải là ảnh còn lại sau khi xóa)</summary>
        public long? MainImageId { get; set; }
    }
}