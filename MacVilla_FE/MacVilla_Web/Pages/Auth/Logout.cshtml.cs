using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            // X�a cookie jwt
            Response.Cookies.Delete("jwt");
            return RedirectToPage("Login");
        }
    }
}
