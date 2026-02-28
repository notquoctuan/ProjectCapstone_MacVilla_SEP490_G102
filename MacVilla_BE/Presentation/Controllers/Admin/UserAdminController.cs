using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserAdminController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Chức năng 1: Lấy danh sách tài khoản có phân trang và tìm kiếm
        /// GET: /api/users?pageNumber=1&pageSize=10&email=abc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserSearchRequest request)
        {
            // Gọi Service để xử lý logic phân trang và tìm kiếm
            var response = await _userService.GetUsersAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Chức năng 2: Thêm mới tài khoản (Admin/Staff tạo cho người dùng)
        /// POST: /api/users
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi Service để hash mật khẩu và lưu vào DB
            var isSuccess = await _userService.CreateAccountAsync(request);

            if (!isSuccess)
            {
                return BadRequest(new { message = "Email đã tồn tại trên hệ thống." });
            }

            return Ok(new { message = "Tạo tài khoản thành công." });
        }
    }
}
