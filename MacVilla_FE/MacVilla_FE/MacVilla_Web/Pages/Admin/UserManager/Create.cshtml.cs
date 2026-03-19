using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MacVilla_Web.Models;
using MacVilla_Web.Services;

namespace MacVilla_Web.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserApiService _userApi;
        public CreateModel(UserApiService userApi) => _userApi = userApi;

        [BindProperty]
        public UserCreateRequest Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Bỏ qua check Role/PositionId nếu dùng input ẩn (vì ModelState có thể ko bắt kịp)
            if (!ModelState.IsValid) return Page();

            var result = await _userApi.AddUserAsync(Input);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Thêm thành viên thành công!";
                return RedirectToPage("./Index");
            }

            ErrorMessage = result.message;
            return Page();
        }
    }
}