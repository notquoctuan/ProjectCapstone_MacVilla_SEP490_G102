using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using BCrypt.Net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // 1. Thực thi View List với Search/Paging (Trả về DTO cho Controller)
        public async Task<PagedResponse<UserResponse>> GetUsersAsync(UserSearchRequest request)
        {
            // Vì Repository chỉ có GetAllAsync, ta lấy toàn bộ rồi xử lý Filter/Paging tại đây
            var allUsers = await _userRepo.GetAllAsync();
            var query = allUsers.AsQueryable();

            // Logic Search/Filter
            if (!string.IsNullOrEmpty(request.Email))
                query = query.Where(u => u.Email.Contains(request.Email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(request.FullName))
                query = query.Where(u => u.FullName != null && u.FullName.Contains(request.FullName, StringComparison.OrdinalIgnoreCase));

            var totalCount = query.Count();

            // Thực hiện phân trang
            var pagedItems = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserResponse
                {
                    Id = u.UserId,
                    Email = u.Email,
                    FullName = u.FullName ?? "N/A",
                    Role = u.Role ?? "Staff",
                    Status = u.Status ?? "Active",
                    // Fix lỗi CS0266: Xử lý gán DateTime? vào DateTime
                    CreatedAt = u.CreatedAt ?? DateTime.Now
                }).ToList();

            return new PagedResponse<UserResponse>
            {
                Data = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        // 2. Thực thi Add New Account (Đã sửa theo quan hệ ICollection)
        public async Task<bool> CreateAccountAsync(CreateUserRequest request)
        {
            // 1. Kiểm tra trùng Email
            var existingEmail = await _userRepo.GetByEmailAsync(request.Email);
            if (existingEmail != null)
                throw new Exception("Email đã tồn tại."); // Bạn có thể dùng Result Pattern thay vì throw

            // 2. Kiểm tra trùng Số điện thoại
            var existingPhone = await _userRepo.GetByPhoneAsync(request.Phone);
            if (existingPhone != null)
                throw new Exception("Số điện thoại đã tồn tại.");

            // 3. Tạo mới nếu hợp lệ
            var newUser = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                Role = request.Role,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            newUser.UserCredentials.Add(new UserCredential
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.Now
            });

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        // 3. Thực thi phương thức phụ trả về Entity
        public async Task<PagedResponse<User>> GetUsersPagedAsync(UserSearchRequest request)
        {
            var allUsers = await _userRepo.GetAllAsync();
            var query = allUsers.AsQueryable();

            var totalCount = query.Count();
            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<User>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
