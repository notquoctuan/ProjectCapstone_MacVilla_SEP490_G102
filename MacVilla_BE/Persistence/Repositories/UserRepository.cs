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

        public UserRepository(MacvilladbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AddCredentialAsync(UserCredential credential)
        {
            await _context.UserCredentials.AddAsync(credential);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
