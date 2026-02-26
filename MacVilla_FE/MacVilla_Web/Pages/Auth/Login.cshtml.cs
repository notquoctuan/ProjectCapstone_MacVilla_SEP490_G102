using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public LoginModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty]
        public LoginRequest Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            var response = await client.PostAsJsonAsync("api/auth/login", Input);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    HttpContext.Session.SetString("JWToken", result.Token);

                    return RedirectToPage("/Admin/Dashboard/Index");
                }
            }

            ErrorMessage = "Tài khoản hoặc mật khẩu Admin không chính xác.";
            return Page();
        }
    }
}
