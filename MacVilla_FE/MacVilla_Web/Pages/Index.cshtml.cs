using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                return RedirectToPage("/Admin/Dashboard/Index");
            return RedirectToPage("/Auth/Login");
        }
    }
}
