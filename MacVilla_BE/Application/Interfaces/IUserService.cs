using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserListResponse>> GetPagedUsersAsync(UserSearchRequest request);
        Task<UserDetailResponse?> GetUserDetailAsync(long id);
        Task<UserListResponse> CreateUserAsync(CreateUserRequest request);
        Task UpdateUserAsync(long id, UpdateUserRequest request);
        Task ChangeStatusAsync(long id, ChangeUserStatusRequest request);
        Task ResetPasswordAsync(long id, ResetPasswordRequest request);
        Task DeleteUserAsync(long id);
    }
}