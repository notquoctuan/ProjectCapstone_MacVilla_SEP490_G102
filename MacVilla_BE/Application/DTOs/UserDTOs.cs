using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    // ── Response ────────────────────────────────────────────────────────

    public class UserListResponse
    {
        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UserDetailResponse
    {
        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Thông tin bổ sung
        public int TotalOrders { get; set; }
        public int TotalFeedbacks { get; set; }
    }

    // ── Request ─────────────────────────────────────────────────────────

    public class UserSearchRequest
    {
        [StringLength(255, ErrorMessage = "Từ khóa tìm kiếm không được vượt quá 255 ký tự.")]
        public string? Keyword { get; set; }    // tìm theo email hoặc họ tên

        [RegularExpression("^(Admin|Customer|Employee)$",
            ErrorMessage = "Role chỉ được là: Admin, Customer, Employee.")]
        public string? Role { get; set; }

        [RegularExpression("^(Active|Disable)$",
            ErrorMessage = "Status chỉ được là: Active, Disable.")]
        public string? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Kích thước trang phải từ 1 đến 100.")]
        public int PageSize { get; set; } = 10;

        [RegularExpression("^(newest|oldest)$",
            ErrorMessage = "SortOrder chỉ được là: newest, oldest.")]
        public string? SortOrder { get; set; } = "newest";
    }

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [MaxLength(255, ErrorMessage = "Họ tên không được vượt quá 255 ký tự.")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [MaxLength(50, ErrorMessage = "Số điện thoại không được vượt quá 50 ký tự.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role không được để trống.")]
        [RegularExpression("^(Admin|Customer|Employee)$",
            ErrorMessage = "Role chỉ được là: Admin, Customer, Employee.")]
        public string Role { get; set; } = "Customer";
    }

    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [MaxLength(255, ErrorMessage = "Họ tên không được vượt quá 255 ký tự.")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [MaxLength(50, ErrorMessage = "Số điện thoại không được vượt quá 50 ký tự.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role không được để trống.")]
        [RegularExpression("^(Admin|Customer|Employee)$",
            ErrorMessage = "Role chỉ được là: Admin, Customer, Employee.")]
        public string Role { get; set; } = null!;
    }

    public class ChangeUserStatusRequest
    {
        [Required(ErrorMessage = "Status không được để trống.")]
        [RegularExpression("^(Active|Disable)$",
            ErrorMessage = "Status chỉ được là: Active hoặc Disable.")]
        public string Status { get; set; } = null!;
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}