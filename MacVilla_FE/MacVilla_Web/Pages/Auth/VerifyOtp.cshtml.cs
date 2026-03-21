using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Auth
{
    public class VerifyOtpModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public VerifyOtpModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Otp { get; set; } = "";

        public string? Message { get; set; }
        public bool IsError { get; set; }

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");

            var res = await client.PostAsJsonAsync("/api/auth/verify-otp",
                new VerifyOtpRequest
                {
                    Email = Email,
                    Otp = Otp
                });

            if (!res.IsSuccessStatusCode)
            {
                Message = "OTP không đúng hoặc đã hết hạn";
                IsError = true;
                return Page();
            }

            // ✅ thành công → về login
            return RedirectToPage("/Auth/Login");
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");

            var res = await client.PostAsJsonAsync("/api/auth/resend-otp",
                new SendOtpRequest
                {
                    Email = Email
                });

            if (!res.IsSuccessStatusCode)
            {
                Message = "Không thể gửi lại OTP";
                IsError = true;
                return Page();
            }

            Message = "OTP mới đã được gửi về email";
            IsError = false;
            return Page();
        }
    }
}
