using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MacVilla_Web.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public CreateModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty]
        public CreateUserRequest CreateRequest { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var response = await client.PostAsJsonAsync("api/admin/users", CreateRequest);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Tạo tài khoản thành công.";
                return RedirectToPage("Index");
            }
            ErrorMessage = $"Lỗi: {await response.Content.ReadAsStringAsync()}";
            return Page();
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }

    public class CreateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = "Customer";
    }
}
