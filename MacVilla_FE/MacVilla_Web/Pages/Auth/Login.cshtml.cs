using MacVilla_Web.Models;
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

        public IActionResult OnGet()
        {
            // Kiểm tra cả cookie lẫn session
            var token = Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token)) return RedirectToPage("/Admin/Dashboard/Index");
            return Page();
        }

        // JS login gọi endpoint này để lưu session server-side sau khi đã xác thực BE
        public IActionResult OnPostSetSession([FromBody] SetSessionRequest req)
        {
            if (string.IsNullOrEmpty(req?.Token))
                return BadRequest(new { message = "Token không hợp lệ." });
            HttpContext.Session.SetString("JWToken", req.Token);
            HttpContext.Session.SetString("UserRole", req.Role ?? "");
            HttpContext.Session.SetString("UserName", req.Name ?? "");
            return new JsonResult(new { success = true });
        }
    }

    public class SetSessionRequest
    {
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? Name { get; set; }
    }
}
