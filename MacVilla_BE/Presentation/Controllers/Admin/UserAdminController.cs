using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    // [Authorize(Roles = "Admin")] // Mở ra khi bạn đã làm xong Auth
    public class UserAdminController : ControllerBase
    {
        private readonly UserService _userService;

        public UserAdminController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                var users = await _userService.GetAllUsersForAdminAsync();

                if (users == null || !users.Any())
                    return NotFound(new { message = "Không có người dùng nào." });

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(request.LoginId) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống." });

            try
            {
                var result = await _userService.AddInternalUserAsync(request);
                return Ok(new
                {
                    message = "Đã tạo tài khoản nội bộ thành công!",
                    user = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] UserFilterRequest filter)
        {
            var result = await _userService.GetAdvancedFilterAsync(filter);
            return Ok(result);
        }
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            try
            {
                var newStatus = await _userService.ToggleUserStatusAsync(id);
                return Ok(new
                {
                    message = $"Đã chuyển trạng thái sang {newStatus}",
                    currentStatus = newStatus
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}