using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductSearchRequest
    {
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|./?]*$",
            ErrorMessage = "Tên tìm kiếm không được chứa ký tự đặc biệt.")]
        public string? Name { get; set; }

        public string? CategoryName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá thấp nhất không được âm")]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá cao nhất không được âm")]
        public decimal? MaxPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Số lượng mỗi trang từ 1 đến 100")]
        public int PageSize { get; set; } = 10;
    }
}
