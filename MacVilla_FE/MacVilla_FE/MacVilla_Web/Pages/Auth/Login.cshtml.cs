using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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
                if (result != null && !string.IsNullOrEmpty(result.TokenValue))
                {
                    HttpContext.Session.SetString("JWToken", result.TokenValue);

                    return RedirectToPage("/Index");
                }
            }

            ErrorMessage = "Tài khoản hoặc mật khẩu không chính xác.";
            return Page();
        }

        // AJAX Login Handler
        public async Task<JsonResult> OnPostAjaxLoginAsync([FromBody] AjaxLoginRequest request)
        {
            try
            {
                var client = _clientFactory.CreateClient("MacVillaAPI");
                var loginRequest = new LoginRequest 
                { 
                    Email = request.Email, 
                    Password = request.Password 
                };
                
                var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.TokenValue))
                    {
                        // Store in server session
                        HttpContext.Session.SetString("JWToken", result.TokenValue);
                        
                        // Determine redirect based on role
                        var redirectUrl = result.Role == "Admin" ? "/Admin/Dashboard/Index" : "/Index";
                        
                        return new JsonResult(new { 
                            success = true, 
                            token = result.TokenValue,
                            redirectUrl = redirectUrl,
                            role = result.Role 
                        });
                    }
                }

                return new JsonResult(new { 
                    success = false, 
                    message = "Tài khoản hoặc mật khẩu không chính xác." 
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Đã xảy ra lỗi: " + ex.Message 
                });
            }
        }
    }

    // Separate class for AJAX request
    public class AjaxLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
