using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> CreateAdminAsync(CreateAdminRequest request);
    Task SendOtpAsync(string email);
    Task ResendOtpAsync(string email);
    Task<LoginResponse> LoginWithGoogleAsync(string idToken);
    Task<bool> VerifyOtpAsync(string email, string otp);
}