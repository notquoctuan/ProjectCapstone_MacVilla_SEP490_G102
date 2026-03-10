//using Application.DTOs;
//using Application.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace Presentation.Controllers.Admin
//{
//    [ApiController]
//    [Route("api/admin/users")]
//    [Authorize(Roles = "Admin")]
//    public class UserAdminController : ControllerBase
//    {
//        private readonly UserService _userService;

//        public UserAdminController(UserService userService)
//        {
//            _userService = userService;
//        }

//        // GET api/admin/users?keyword=&role=&status=&pageNumber=1&pageSize=10
//        [HttpGet]
//        public async Task<IActionResult> GetUsers([FromQuery] UserSearchRequest request)
//        {
//            try
//            {
//                var result = await _userService.GetPagedUsersAsync(request);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // GET api/admin/users/{id}
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetUserDetail(long id)
//        {
//            try
//            {
//                var user = await _userService.GetUserDetailAsync(id);
//                if (user == null)
//                    return NotFound(new { message = "Không tìm thấy người dùng." });
//                return Ok(user);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // POST api/admin/users
//        [HttpPost]
//        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(FormatErrors());

//            try
//            {
//                var result = await _userService.CreateUserAsync(request);
//                return CreatedAtAction(nameof(GetUserDetail), new { id = result.UserId },
//                    new { message = "Tạo tài khoản thành công.", user = result });
//            }
//            catch (InvalidOperationException ex)
//            {
//                return Conflict(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // PUT api/admin/users/{id}
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserRequest request)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(FormatErrors());

//            try
//            {
//                await _userService.UpdateUserAsync(id, request);
//                return Ok(new { message = "Cập nhật người dùng thành công." });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // PATCH api/admin/users/{id}/status
//        [HttpPatch("{id}/status")]
//        public async Task<IActionResult> ChangeStatus(long id, [FromBody] ChangeUserStatusRequest request)
//        {
//            try
//            {
//                await _userService.ChangeStatusAsync(id, request);
//                return Ok(new { message = $"Đã cập nhật trạng thái sang '{request.Status}'." });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (ArgumentException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // PATCH api/admin/users/{id}/reset-password
//        [HttpPatch("{id}/reset-password")]
//        public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordRequest request)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(FormatErrors());

//            try
//            {
//                await _userService.ResetPasswordAsync(id, request);
//                return Ok(new { message = "Đặt lại mật khẩu thành công." });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (ArgumentException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // DELETE api/admin/users/{id}
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteUser(long id)
//        {
//            try
//            {
//                await _userService.DeleteUserAsync(id);
//                return Ok(new { message = "Xóa người dùng thành công." });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
//            }
//        }

//        // ── Helper ──────────────────────────────────────────────────────
//        private object FormatErrors() => new
//        {
//            message = "Dữ liệu không hợp lệ.",
//            errors = ModelState
//                .Where(x => x.Value?.Errors.Count > 0)
//                .ToDictionary(
//                    x => x.Key,
//                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
//                )
//        };
//    }
//}