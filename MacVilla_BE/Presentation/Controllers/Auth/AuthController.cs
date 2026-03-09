using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    /// <summary>Đăng nhập — trả về JWT token</summary>
    [HttpPost("login")]
    [EnableRateLimiting("LoginPolicy")] // max 5 lần/phút/IP
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                message = "Dữ liệu không hợp lệ.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList())
            });

        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// CHỈ DÙNG KHI DEVELOPMENT — Tạo tài khoản nhanh.
    /// Endpoint này sẽ trả về 404 ở môi trường Production.
    /// </summary>
    [HttpPost("dev/create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
    {
        if (!_env.IsDevelopment())
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest(new
            {
                message = "Dữ liệu không hợp lệ.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList())
            });

        try
        {
            var result = await _authService.CreateAdminAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}