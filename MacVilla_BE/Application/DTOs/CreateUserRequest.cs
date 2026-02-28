using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email phải có đuôi @gmail.com")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Mật khẩu phải bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải có 10-11 chữ số.")]
        public string Phone { get; set; } = null!;

        public string Role { get; set; } = "Inventory manager";
    }
}
