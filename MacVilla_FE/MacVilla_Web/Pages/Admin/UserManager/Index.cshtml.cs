using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MacVilla_Web.Models;
using MacVilla_Web.Services;

namespace MacVilla_Web.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserApiService _userApi;
        public IndexModel(UserApiService userApi) => _userApi = userApi;

        public List<UserAdminVM> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)] public string? Role { get; set; }
        [BindProperty(SupportsGet = true)] public string? Status { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userApi.GetUsersAsync(SearchTerm, Role, Status);
        }

        public async Task<JsonResult> OnPostToggleStatusAsync(long id)
        {
            var result = await _userApi.ToggleStatusAsync(id);
            return new JsonResult(new { success = result });
        }
    }
}