using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
            => _userService = userService;

        private long CurrentUserId =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // ────────────────────────────────────────────────────────────────
        /// <summary>Lấy danh sách user (lọc + phân trang)</summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserSearchRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                var result = await _userService.GetPagedUsersAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ────────────────────────────────────────────────────────────────
        /// <summary>Lấy chi tiết 1 user</summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDetail(long id)
        {
            try
            {
                var user = await _userService.GetUserDetailAsync(id);
                if (user == null) return NotFound(new { message = "Không tìm thấy người dùng." });
                return Ok(user);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ────────────────────────────────────────────────────────────────
        /// <summary>Tạo tài khoản mới (Admin tạo hộ)</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                var result = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetDetail), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        // ────────────────────────────────────────────────────────────────
        /// <summary>Cập nhật thông tin user (FullName, Phone, Role)</summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                await _userService.UpdateUserAsync(id, request);
                return Ok(new { message = "Cập nhật thông tin người dùng thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ────────────────────────────────────────────────────────────────
        /// <summary>Kích hoạt / vô hiệu hóa tài khoản</summary>
        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] ChangeUserStatusRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());

            // Không cho tự khóa chính mình
            if (id == CurrentUserId)
                return BadRequest(new { message = "Không thể thay đổi trạng thái tài khoản của chính mình." });

            try
            {
                await _userService.ChangeStatusAsync(id, request);
                return Ok(new { message = $"Đã chuyển trạng thái tài khoản sang '{request.Status}'." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ────────────────────────────────────────────────────────────────
        /// <summary>Reset mật khẩu người dùng (Admin đặt lại hộ)</summary>
        [HttpPatch("{id:long}/reset-password")]
        public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                await _userService.ResetPasswordAsync(id, request);
                return Ok(new { message = "Đặt lại mật khẩu thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ────────────────────────────────────────────────────────────────
        /// <summary>Xóa tài khoản (chỉ được xóa nếu không có đơn hàng)</summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            // Không cho tự xóa chính mình
            if (id == CurrentUserId)
                return BadRequest(new { message = "Không thể xóa tài khoản của chính mình." });

            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { message = "Xóa tài khoản thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── Helper ──────────────────────────────────────────────────────
        private object FormatErrors() => new
        {
            message = "Dữ liệu không hợp lệ.",
            errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                )
        };
    }
}