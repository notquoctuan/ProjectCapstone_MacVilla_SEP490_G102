using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public bool IsAuthenticated { get; set; }

        public void OnGet()
        {
            var token = HttpContext.Session.GetString("JWToken");
            IsAuthenticated = !string.IsNullOrEmpty(token);
        }
    }
}
