using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Google.Apis.Auth;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IMemoryCache _cache;
    private readonly IUserOauthRepository _userOauthRepo;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepo, ITokenService tokenService, IMemoryCache cache,
    IUserOauthRepository userOauthRepo, IEmailService emailService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _cache = cache;
        _userOauthRepo = userOauthRepo;
        _emailService = emailService;
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

    public async Task SendOtpAsync(string email)
    {
        email = email.Trim().ToLower();

        var user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
            throw new Exception("Email chưa đăng ký");

        var cooldownKey = $"otp_cd_{email}";
        var otpKey = $"otp_{email}";

        // check cooldown 60s
        if (_cache.TryGetValue(cooldownKey, out _))
            throw new Exception("Vui lòng đợi 60 giây trước khi yêu cầu mã mới.");

        var otp = new Random().Next(100000, 999999).ToString();

        // lưu OTP 5 phút
        _cache.Set(otpKey, otp, TimeSpan.FromMinutes(5));

        // lưu cooldown 60s
        _cache.Set(cooldownKey, true, TimeSpan.FromSeconds(60));

        await _emailService.SendOtpEmailAsync(email, otp);
    }

    public async Task ResendOtpAsync(string email)
    {
        await SendOtpAsync(email);
    }
    public async Task<LoginResponse> LoginWithGoogleAsync(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        var email = payload.Email;
        var googleId = payload.Subject;
        var name = payload.Name;

        // 1. Check OAuth
        var oauth = await _userOauthRepo
            .GetByProviderAndProviderId("Google", googleId);

        if (oauth != null)
        {
            var user = oauth.User;

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse(
                user.UserId,
                user.Email,
                user.FullName ?? "",
                user.Role ?? "",
                token
            );
        }

        // 2. Check user
        var userByEmail = await _userRepo.GetByEmailAsync(email);

        if (userByEmail == null)
        {
            userByEmail = await _userRepo.CreateUserAsync(
                new User
                {
                    Email = email,
                    FullName = name,
                    Role = "Customer",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                },
                Guid.NewGuid().ToString() // fake password
            );
        }

        // 3. Save OAuth
        await _userOauthRepo.CreateAsync(new UserOauth
        {
            UserId = userByEmail.UserId,
            Provider = "Google",
            ProviderUserId = googleId
        });

        // 4. Return token
        var jwt = _tokenService.GenerateToken(userByEmail);

        return new LoginResponse(
            userByEmail.UserId,
            userByEmail.Email,
            userByEmail.FullName ?? "",
            userByEmail.Role ?? "",
            jwt
        );
    }

    public Task<bool> VerifyOtpAsync(string email, string otp)
    {
        var key = $"otp_{email.Trim().ToLower()}";

        if (!_cache.TryGetValue(key, out string cachedOtp))
            return Task.FromResult(false);

        if (cachedOtp != otp)
            return Task.FromResult(false);

        // ❗ OTP đúng → xóa luôn (one-time)
        _cache.Remove(key);

        return Task.FromResult(true);
    }
}