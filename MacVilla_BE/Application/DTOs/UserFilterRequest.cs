using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserFilterRequest
    {
        public string? SearchTerm { get; set; }

        [RegularExpression("Admin|Employee|Customer", ErrorMessage = "Vai trò không hợp lệ")]
        public string? Role { get; set; }

        [RegularExpression("Active|Disable", ErrorMessage = "Trạng thái không hợp lệ")]
        public string? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}