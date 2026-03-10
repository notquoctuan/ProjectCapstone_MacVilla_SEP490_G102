using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MacvilladbContext _ctx;

        public UserRepository(MacvilladbContext ctx) => _ctx = ctx;

        // ── Auth ──────────────────────────────────────────────────────
        public async Task<User?> GetByEmailAsync(string email)
            => await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<UserCredential?> GetCredentialAsync(long userId)
            => await _ctx.UserCredentials.FirstOrDefaultAsync(c => c.UserId == userId);

        public async Task<User> CreateUserAsync(User user, string passwordHash)
        {
            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();

            _ctx.UserCredentials.Add(new UserCredential
            {
                UserId = user.UserId,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            });
            await _ctx.SaveChangesAsync();
            return user;
        }

        // ── User Management ───────────────────────────────────────────
        public IQueryable<User> GetQueryable()
            => _ctx.Users.AsNoTracking();

        public async Task<User?> GetByIdAsync(long id)
            => await _ctx.Users
                .Include(u => u.Orders)
                .Include(u => u.Feedbacks)
                .FirstOrDefaultAsync(u => u.UserId == id);

        public async Task<bool> ExistsByEmailAsync(string email, long? excludeId = null)
        {
            var query = _ctx.Users.Where(u => u.Email == email.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(u => u.UserId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _ctx.Users.Update(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatusAsync(long id, string status)
        {
            var user = await _ctx.Users.FindAsync(id);
            if (user == null) return false;
            user.Status = status;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task ResetPasswordAsync(long userId, string newPasswordHash)
        {
            var credential = await _ctx.UserCredentials
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (credential == null)
            {
                // Tạo mới nếu chưa có (trường hợp user OAuth)
                _ctx.UserCredentials.Add(new UserCredential
                {
                    UserId = userId,
                    PasswordHash = newPasswordHash,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                credential.PasswordHash = newPasswordHash;
            }

            await _ctx.SaveChangesAsync();
        }
    }
}