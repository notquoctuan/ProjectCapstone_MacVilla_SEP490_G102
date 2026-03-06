using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        // Auth (giữ nguyên)
        Task<User?> GetByEmailAsync(string email);
        Task<UserCredential?> GetCredentialAsync(long userId);
        Task<User> CreateUserAsync(User user, string passwordHash);

        // User Management
        IQueryable<User> GetQueryable();
        Task<User?> GetByIdAsync(long id);
        Task<bool> ExistsByEmailAsync(string email, long? excludeId = null);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> UpdateStatusAsync(long id, string status);
        Task ResetPasswordAsync(long userId, string newPasswordHash);
    }
}