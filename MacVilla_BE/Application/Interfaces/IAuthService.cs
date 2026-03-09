using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> CreateAdminAsync(CreateAdminRequest request);
}