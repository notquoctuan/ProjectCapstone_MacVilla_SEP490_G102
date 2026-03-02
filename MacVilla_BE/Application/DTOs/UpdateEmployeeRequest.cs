using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UpdateEmployeeRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\d{10,11}$",
            ErrorMessage = "Số điện thoại phải có 10-11 chữ số.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Chức vụ là bắt buộc.")]
        public string Position { get; set; } = null!;
    }
}
