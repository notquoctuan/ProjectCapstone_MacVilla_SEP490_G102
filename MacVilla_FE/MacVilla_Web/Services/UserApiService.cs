using MacVilla_Web.Models;
using System.Net.Http.Headers;
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

        private void SetAuthHeader(string? token)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        // Lấy danh sách paged - dùng đúng endpoint GET /api/admin/users
        public async Task<PagedResponse<UserAdminVM>> GetPagedUsersAsync(
            string? keyword, string? role, string? status,
            int pageNumber = 1, int pageSize = 15, string? token = null)
        {
            try
            {
                SetAuthHeader(token);
                var parts = new List<string> { $"PageNumber={pageNumber}", $"PageSize={pageSize}" };
                if (!string.IsNullOrEmpty(keyword)) parts.Add($"Keyword={Uri.EscapeDataString(keyword)}");
                if (!string.IsNullOrEmpty(role)) parts.Add($"Role={Uri.EscapeDataString(role)}");
                if (!string.IsNullOrEmpty(status)) parts.Add($"Status={Uri.EscapeDataString(status)}");

                var response = await _httpClient.GetFromJsonAsync<PagedResponse<UserAdminVM>>(
                    $"api/admin/users?{string.Join("&", parts)}");
                return response ?? new PagedResponse<UserAdminVM>();
            }
            catch { return new PagedResponse<UserAdminVM>(); }
        }

        // Tạo user - POST /api/admin/users
        public async Task<(bool success, string message)> AddUserAsync(
            CreateUserAdminRequest request, string? token = null)
        {
            try
            {
                SetAuthHeader(token);
                var response = await _httpClient.PostAsJsonAsync("api/admin/users", request);
                if (response.IsSuccessStatusCode) return (true, "Thành công");
                var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, err?.message ?? err?.Message ?? "Lỗi không xác định");
            }
            catch (Exception ex) { return (false, "Lỗi kết nối: " + ex.Message); }
        }

        // Cập nhật trạng thái - PATCH /api/admin/users/{id}/status
        public async Task<bool> ChangeStatusAsync(long id, string newStatus, string? token = null)
        {
            try
            {
                SetAuthHeader(token);
                var response = await _httpClient.PatchAsJsonAsync(
                    $"api/admin/users/{id}/status", new { Status = newStatus });
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public class ApiErrorResponse
        {
            public string? message { get; set; }
            public string? Message { get; set; }
        }
    }

    // DTO khớp với BE CreateUserRequest
    public class CreateUserAdminRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = "Customer";
    }
}
