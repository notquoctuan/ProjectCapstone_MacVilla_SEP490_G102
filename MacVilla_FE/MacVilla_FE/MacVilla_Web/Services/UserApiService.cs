using MacVilla_Web.Models;
using System.Net.Http.Json;

namespace MacVilla_Web.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Lấy danh sách và Filter
        public async Task<List<UserAdminVM>> GetUsersAsync(string? searchTerm, string? role, string? status)
        {
            try
            {
                // Khớp với [HttpGet("filter")] của BE
                var query = $"api/admin/users/filter?SearchTerm={searchTerm}&Role={role}&Status={status}";
                var response = await _httpClient.GetFromJsonAsync<List<UserAdminVM>>(query);
                return response ?? new List<UserAdminVM>();
            }
            catch { return new List<UserAdminVM>(); }
        }

        // 2. Toggle Status (Khóa/Mở khóa)
        public async Task<bool> ToggleStatusAsync(long id)
        {
            // Khớp với [HttpPatch("{id}/toggle-status")] của BE
            var response = await _httpClient.PatchAsync($"api/admin/users/{id}/toggle-status", null);
            return response.IsSuccessStatusCode;    
        }

        // Thêm class này vào cuối file UserApiService.cs hoặc trong thư mục Models
        public class ApiErrorResponse
        {
            public string? message { get; set; }
        }

        // Trong hàm AddUserAsync:
        public async Task<(bool success, string message)> AddUserAsync(UserCreateRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/admin/users/add-user", request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thành công");
                }

                // Đọc thông báo lỗi từ Backend (Ví dụ: "Tên đăng nhập đã tồn tại")
                var errorData = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, errorData?.message ?? "Lỗi không xác định từ máy chủ.");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi kết nối: " + ex.Message);
            }
        }
    }
}