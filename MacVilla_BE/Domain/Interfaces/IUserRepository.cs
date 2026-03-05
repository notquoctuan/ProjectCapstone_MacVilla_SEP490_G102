using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersWithRoleAsync();
        Task<User?> GetByIdAsync(long id);
        IQueryable<User> GetQueryable();
        Task<User> AddAsync(User user); // Thêm mới
        Task<bool> IsEmailExistsAsync(string email); // Kiểm tra trùng LoginId/Email
        Task<bool> UpdateStatusAsync(long userId, string newStatus);
    }
}
