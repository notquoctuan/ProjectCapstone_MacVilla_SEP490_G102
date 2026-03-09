using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
            => _userRepo = userRepo;

        // ────────────────────────────────────────────────────────────────
        // GET PAGED
        // ────────────────────────────────────────────────────────────────
        public async Task<PagedResponse<UserListResponse>> GetPagedUsersAsync(UserSearchRequest request)
        {
            var query = _userRepo.GetQueryable();

            // Tìm theo email hoặc họ tên
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim();
                query = query.Where(u =>
                    (u.Email != null && u.Email.Contains(kw)) ||
                    (u.FullName != null && u.FullName.Contains(kw)));
            }

            // Lọc theo role
            if (!string.IsNullOrWhiteSpace(request.Role))
                query = query.Where(u => u.Role == request.Role);

            // Lọc theo status
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(u => u.Status == request.Status);

            // Sắp xếp
            query = request.SortOrder == "oldest"
                ? query.OrderBy(u => u.CreatedAt)
                : query.OrderByDescending(u => u.CreatedAt);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserListResponse
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    Role = u.Role,
                    Status = u.Status,
                    Avatar = u.Avatar,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return new PagedResponse<UserListResponse>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }

        // ────────────────────────────────────────────────────────────────
        // GET DETAIL
        // ────────────────────────────────────────────────────────────────
        public async Task<UserDetailResponse?> GetUserDetailAsync(long id)
        {
            if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.");

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDetailResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role,
                Status = user.Status,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,
                TotalOrders = user.Orders.Count,
                TotalFeedbacks = user.Feedbacks.Count
            };
        }

        // ────────────────────────────────────────────────────────────────
        // CREATE
        // ────────────────────────────────────────────────────────────────
        public async Task<UserListResponse> CreateUserAsync(CreateUserRequest request)
        {
            var email = request.Email.Trim().ToLower();

            if (await _userRepo.ExistsByEmailAsync(email))
                throw new InvalidOperationException($"Email '{request.Email}' đã được sử dụng.");

            var user = await _userRepo.CreateUserAsync(
                new User
                {
                    Email = email,
                    FullName = request.FullName.Trim(),
                    Phone = request.Phone?.Trim(),
                    Role = request.Role,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                },
                BCrypt.Net.BCrypt.HashPassword(request.Password)
            );

            return MapToListResponse(user);
        }

        // ────────────────────────────────────────────────────────────────
        // UPDATE
        // ────────────────────────────────────────────────────────────────
        public async Task UpdateUserAsync(long id, UpdateUserRequest request)
        {
            if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.");

            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            user.FullName = request.FullName.Trim();
            user.Phone = request.Phone?.Trim();
            user.Role = request.Role;

            await _userRepo.UpdateAsync(user);
        }

        // ────────────────────────────────────────────────────────────────
        // CHANGE STATUS
        // ────────────────────────────────────────────────────────────────
        public async Task ChangeStatusAsync(long id, ChangeUserStatusRequest request)
        {
            if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.");

            var updated = await _userRepo.UpdateStatusAsync(id, request.Status);
            if (!updated)
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        // ────────────────────────────────────────────────────────────────
        // RESET PASSWORD
        // ────────────────────────────────────────────────────────────────
        public async Task ResetPasswordAsync(long id, ResetPasswordRequest request)
        {
            if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.");

            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepo.ResetPasswordAsync(user.UserId, newHash);
        }

        // ────────────────────────────────────────────────────────────────
        // DELETE
        // ────────────────────────────────────────────────────────────────
        public async Task DeleteUserAsync(long id)
        {
            if (id <= 0) throw new ArgumentException("ID người dùng không hợp lệ.");

            var user = await _userRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            // Không cho xóa nếu có đơn hàng
            if (user.Orders.Any())
                throw new InvalidOperationException("Không thể xóa người dùng đã có đơn hàng. Hãy vô hiệu hóa thay vì xóa.");

            await _userRepo.DeleteAsync(user);
        }

        // ── Helper ───────────────────────────────────────────────────
        private static UserListResponse MapToListResponse(User u) => new()
        {
            UserId = u.UserId,
            Email = u.Email,
            FullName = u.FullName,
            Phone = u.Phone,
            Role = u.Role,
            Status = u.Status,
            Avatar = u.Avatar,
            CreatedAt = u.CreatedAt
        };
    }
}