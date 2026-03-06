using System.Net.Http.Headers;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Categories
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public CreateModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty]
        public CreateCategoryRequest CreateRequest { get; set; } = new();
        public List<Category>? ParentCategories { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");
            await LoadParentCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            if (string.IsNullOrWhiteSpace(CreateRequest.CategoryName))
            {
                ModelState.AddModelError("CreateRequest.CategoryName", "Tên danh mục không được để trống.");
                await LoadParentCategoriesAsync();
                return Page();
            }

            var client = CreateAuthenticatedClient(token);
            var response = await client.PostAsJsonAsync("api/admin/category", CreateRequest);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Thêm danh mục thành công.";
                return RedirectToPage("Index");
            }

            ErrorMessage = "Không thể tạo danh mục. Vui lòng thử lại.";
            await LoadParentCategoriesAsync();
            return Page();
        }

        private async Task LoadParentCategoriesAsync()
        {
            var token = GetToken() ?? "";
            var client = CreateAuthenticatedClient(token);
            var response = await client.GetAsync("api/admin/category?pageNumber=1&pageSize=1000");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MacVilla_Web.Models.PagedResponse<Category>>();
                ParentCategories = result?.Data?.Where(c => !c.ParentCategoryId.HasValue).ToList() ?? new();
            }
            else ParentCategories = new List<Category>();
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
