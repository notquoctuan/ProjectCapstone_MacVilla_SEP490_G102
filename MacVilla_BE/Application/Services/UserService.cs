using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; // ⭐ Thêm dòng này
namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserAdminResponse>> GetAllUsersForAdminAsync()
        {
            var users = await _userRepository.GetAllUsersWithRoleAsync();

            return users.Select(u => new UserAdminResponse
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                // Lấy Position từ danh sách Employees (quan hệ 1-n hoặc 1-1)
                Position = u.Employees.FirstOrDefault()?.Position ?? "N/A"
            });
        }
        public async Task<UserAdminResponse> AddInternalUserAsync(UserCreateRequest request)
        {
            // 1. Kiểm tra ID đăng nhập đã tồn tại chưa
            if (await _userRepository.IsEmailExistsAsync(request.LoginId))
                throw new Exception("Tên đăng nhập/Email này đã tồn tại trong hệ thống.");

            // 2. Khởi tạo thực thể User (Sử dụng LoginId cho cột Email trong DB)
            var user = new User
            {
                Email = request.LoginId,
                FullName = request.FullName,
                Role = request.Role,
                Status = "Active", // Tạo xong dùng được ngay
                CreatedAt = DateTime.Now
            };

            // 3. Hash mật khẩu (Dùng BCrypt)
            user.UserCredentials.Add(new UserCredential
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.Now
            });

            // 4. Map PositionId sang tên chức vụ để lưu vào bảng Employees
            if (request.PositionId.HasValue)
            {
                string positionName = MapPositionIdToName(request.PositionId.Value);
                user.Employees.Add(new Employee { Position = positionName });
            }

            // 5. Lưu xuống Database thông qua Repository
            var result = await _userRepository.AddAsync(user);

            return new UserAdminResponse
            {
                UserId = result.UserId,
                Email = result.Email,
                FullName = result.FullName,
                Position = user.Employees.FirstOrDefault()?.Position
            };
        }
        public async Task<string> ToggleUserStatusAsync(long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Không tìm thấy người dùng.");

            // Logic đảo trạng thái
            string newStatus = (user.Status == "Active") ? "Disable" : "Active";

            var result = await _userRepository.UpdateStatusAsync(userId, newStatus);
            if (!result) throw new Exception("Cập nhật trạng thái thất bại.");

            return newStatus; // Trả về trạng thái mới để FE cập nhật UI ngay lập tức
        }
        public async Task<IEnumerable<UserAdminResponse>> GetAdvancedFilterAsync(UserFilterRequest filter)
        {
            var query = _userRepository.GetQueryable().Include(u => u.Employees).AsNoTracking();

            // Lọc theo từ khóa (Trim và ToLower để tìm kiếm chính xác hơn)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var s = filter.SearchTerm.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(s) || u.Email.ToLower().Contains(s));
            }

            // Lọc theo Role/Status
            if (!string.IsNullOrEmpty(filter.Role)) query = query.Where(u => u.Role == filter.Role);
            if (!string.IsNullOrEmpty(filter.Status)) query = query.Where(u => u.Status == filter.Status);

            // Lọc theo thời gian (Thực tế: dùng .Date để so sánh chính xác ngày)
            if (filter.FromDate.HasValue)
                query = query.Where(u => u.CreatedAt.Value.Date >= filter.FromDate.Value.Date);

            if (filter.ToDate.HasValue)
                query = query.Where(u => u.CreatedAt.Value.Date <= filter.ToDate.Value.Date);

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();

            return users.Select(u => new UserAdminResponse
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName,
                Status = u.Status,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                Position = u.Employees.FirstOrDefault()?.Position ?? (u.Role == "Customer" ? "Khách hàng" : "Chưa rõ")
            });
        }
        private string MapPositionIdToName(int id) => id switch
        {
            1 => "Tổng quản lý",
            2 => "Quản lý kho vận",
            3 => "Nhân viên bán hàng",
            _ => "Nhân viên nội bộ"
        };
    }
}