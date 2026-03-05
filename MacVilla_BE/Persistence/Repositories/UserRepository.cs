using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MacvilladbContext _context;

        public UserRepository(MacvilladbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersWithRoleAsync()
        {
            return await _context.Users
                .Include(u => u.Employees)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public IQueryable<User> GetQueryable()
        {
            return _context.Users.AsNoTracking();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<bool> UpdateStatusAsync(long userId, string newStatus)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
