using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductSearchRequest
    {
        [StringLength(200, ErrorMessage = "Tên tìm kiếm không được vượt quá 200 ký tự.")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Tên danh mục không được vượt quá 200 ký tự.")]
        public string? CategoryName { get; set; }

        public long? CategoryId { get; set; }

        [Range(0, 999_999_999, ErrorMessage = "Giá tối thiểu không hợp lệ.")]
        public decimal? MinPrice { get; set; }

        [Range(0, 999_999_999, ErrorMessage = "Giá tối đa không hợp lệ.")]
        public decimal? MaxPrice { get; set; }

        [RegularExpression("^(Enable|Disable|Pending)$",
            ErrorMessage = "Status chỉ được là: Enable, Disable hoặc Pending.")]
        public string? Status { get; set; }

        [RegularExpression("^(price_asc|price_desc|newest|oldest)$",
            ErrorMessage = "SortOrder chỉ được là: price_asc, price_desc, newest, oldest.")]
        public string? SortOrder { get; set; } = "newest";

        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Kích thước trang phải từ 1 đến 100.")]
        public int PageSize { get; set; } = 10;
    }
}