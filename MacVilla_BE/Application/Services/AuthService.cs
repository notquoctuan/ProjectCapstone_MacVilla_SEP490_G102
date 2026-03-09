using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 1. Validate input thủ công (bổ sung thêm ngoài DataAnnotations)
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email không được để trống.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Mật khẩu không được để trống.");

        // 2. Tìm user theo email
        var user = await _userRepo.GetByEmailAsync(request.Email.Trim().ToLower())
                   ?? throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        // 3. Kiểm tra trạng thái tài khoản
        if (user.Status?.ToLower() != "active")
            throw new UnauthorizedAccessException("Tài khoản đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");

        // 4. Lấy password hash
        var credential = await _userRepo.GetCredentialAsync(user.UserId)
                         ?? throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        // 5. Verify BCrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, credential.PasswordHash))
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        // 6. Generate JWT
        var token = _tokenService.GenerateToken(user);

        return new LoginResponse(
            user.UserId,
            user.Email,
            user.FullName ?? "",
            user.Role ?? "",
            token
        );
    }

    public async Task<LoginResponse> CreateAdminAsync(CreateAdminRequest request)
    {
        // Kiểm tra email đã tồn tại chưa
        var existing = await _userRepo.GetByEmailAsync(request.Email.Trim().ToLower());
        if (existing is not null)
            throw new InvalidOperationException($"Email '{request.Email}' đã tồn tại.");

        // Tạo user + credential
        var user = await _userRepo.CreateUserAsync(
            new User
            {
                Email = request.Email.Trim().ToLower(),
                FullName = request.FullName.Trim(),
                Role = request.Role,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            },
            BCrypt.Net.BCrypt.HashPassword(request.Password)
        );

        var token = _tokenService.GenerateToken(user);

        return new LoginResponse(
            user.UserId,
            user.Email,
            user.FullName ?? "",
            user.Role ?? "",
            token
        );
    }
}