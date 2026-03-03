using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UserCreateRequest
    {[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    public string LoginId { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Họ và tên không được để trống")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vai trò không được để trống.")]   
 public string? Role { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
    public int? PositionId { get; set; }
    }
}