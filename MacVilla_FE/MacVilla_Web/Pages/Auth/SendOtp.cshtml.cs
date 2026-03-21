using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace MacVilla_Web.Pages.Auth
{
    public class SendOtpModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public SendOtpModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        public string? Message { get; set; }
        public bool IsError { get; set; }

        public void OnGet() { }

        // ✅ SEND OTP
        public async Task<IActionResult> OnPostSendOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Vui lòng nhập email";
                IsError = true;
                return Page();
            }

            var client = _clientFactory.CreateClient("MacVillaAPI");

            var body = new SendOtpRequest
            {
                Email = Email.Trim()
            };

            var res = await client.PostAsync("/api/auth/send-otp",
                new StringContent(JsonSerializer.Serialize(body),
                Encoding.UTF8, "application/json"));

            if (!res.IsSuccessStatusCode)
            {
                Message = "Gửi OTP thất bại";
                IsError = true;
                return Page();
            }

            // 👉 chuyển sang trang nhập OTP
            return RedirectToPage("/Auth/VerifyOtp", new { email = Email });
        }

        
    }

}
