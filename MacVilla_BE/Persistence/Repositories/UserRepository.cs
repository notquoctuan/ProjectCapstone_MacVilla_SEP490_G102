using Domain.Entities;
using Domain.Interfaces;
using Persistence.Context;
using Microsoft.EntityFrameworkCore; // Quan trọng: Thêm dòng này để nhận diện các hàm Async
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MacvilladbContext _context;
        public UserRepository(MacvilladbContext context) => _context = context;

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        public async Task<User?> GetByPhoneAsync(string phone)
        => await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);

        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
