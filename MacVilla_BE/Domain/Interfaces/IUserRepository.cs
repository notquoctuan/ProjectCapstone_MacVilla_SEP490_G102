using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone); // Thêm hàm này để tránh trùng số đt
        Task AddAsync(User user);

        //Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}
