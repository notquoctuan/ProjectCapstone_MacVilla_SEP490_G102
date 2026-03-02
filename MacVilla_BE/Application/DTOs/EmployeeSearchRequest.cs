using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class EmployeeSearchRequest
    {
        public string? Name { get; set; }
        public string? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber phải lớn hơn 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 1000, ErrorMessage = "PageSize phải từ 1 đến 1000.")]
        public int PageSize { get; set; } = 10;

        // Nếu true → bỏ qua phân trang và trả toàn bộ dữ liệu
        public bool GetAll { get; set; } = false;
    }
}
