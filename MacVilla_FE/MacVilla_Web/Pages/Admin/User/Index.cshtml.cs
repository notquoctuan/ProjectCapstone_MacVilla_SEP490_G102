using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using MacVilla_Web.Models;

namespace MacVilla_Web.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        // BUG FIX: BE dùng Keyword (tìm cả email + fullname), không có FullName/Email riêng
        [BindProperty(SupportsGet = true)] public string? Keyword { get; set; }
        [BindProperty(SupportsGet = true)] public string? Role { get; set; }
        [BindProperty(SupportsGet = true)] public string? Status { get; set; }
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 15;

        public PagedResponse<UserItemVM>? PagedResult { get; set; }
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var parts = new List<string>();
            // BUG FIX: sử dụng Keyword thay vì FullName + Email riêng
            if (!string.IsNullOrEmpty(Keyword)) parts.Add($"Keyword={Uri.EscapeDataString(Keyword)}");
            if (!string.IsNullOrEmpty(Role)) parts.Add($"Role={Uri.EscapeDataString(Role)}");
            if (!string.IsNullOrEmpty(Status)) parts.Add($"Status={Uri.EscapeDataString(Status)}");
            parts.Add($"PageNumber={PageNumber}");
            parts.Add($"PageSize={PageSize}");

            var response = await client.GetAsync($"api/admin/users?{string.Join("&", parts)}");
            PagedResult = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<PagedResponse<UserItemVM>>()
                : new PagedResponse<UserItemVM>();

            Message = TempData["Message"]?.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(long id, string status)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PatchAsJsonAsync($"api/admin/users/{id}/status", new { Status = status });
            TempData["Message"] = "Đã cập nhật trạng thái tài khoản.";
            return RedirectToPage(new { Keyword, Role, Status, PageNumber, PageSize });
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            var res = await client.DeleteAsync($"api/admin/users/{id}");
            TempData["Message"] = res.IsSuccessStatusCode ? "Xóa tài khoản thành công." : "Không thể xóa tài khoản này.";
            return RedirectToPage(new { Keyword, Role, Status, PageNumber, PageSize });
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(long id, string newPassword, string confirmPassword)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PatchAsJsonAsync($"api/admin/users/{id}/reset-password", new { NewPassword = newPassword, ConfirmPassword = confirmPassword });
            TempData["Message"] = "Đã reset mật khẩu thành công.";
            return RedirectToPage(new { Keyword, Role, Status, PageNumber, PageSize });
        }

        public Dictionary<string, string> GetPageRouteData(int page)
        {
            var d = new Dictionary<string, string> { ["PageNumber"] = page.ToString(), ["PageSize"] = PageSize.ToString() };
            if (!string.IsNullOrEmpty(Keyword)) d["Keyword"] = Keyword;
            if (!string.IsNullOrEmpty(Role)) d["Role"] = Role;
            if (!string.IsNullOrEmpty(Status)) d["Status"] = Status;
            return d;
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }

    public class UserItemVM
    {
        public long UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
