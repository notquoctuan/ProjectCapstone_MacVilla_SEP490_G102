using System.Net.Http.Headers;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Categories
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public EditModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty(SupportsGet = true)] public long Id { get; set; }
        [BindProperty] public UpdateCategoryRequest UpdateRequest { get; set; } = new();
        public List<Category>? ParentCategories { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var response = await client.GetAsync($"api/admin/category/{Id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var category = await response.Content.ReadFromJsonAsync<Category>();
            if (category == null) return NotFound();

            UpdateRequest = new UpdateCategoryRequest
            {
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId
            };
            await LoadParentCategoriesAsync(token);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            if (string.IsNullOrWhiteSpace(UpdateRequest.CategoryName))
            {
                ModelState.AddModelError("UpdateRequest.CategoryName", "Tên danh mục không được để trống.");
                await LoadParentCategoriesAsync(token);
                return Page();
            }

            var client = CreateAuthenticatedClient(token);
            var response = await client.PutAsJsonAsync($"api/admin/category/{Id}", UpdateRequest);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Cập nhật danh mục thành công.";
                return RedirectToPage("Index");
            }

            ErrorMessage = response.StatusCode == System.Net.HttpStatusCode.BadRequest
                ? "Dữ liệu không hợp lệ. Danh mục không thể là cha của chính nó."
                : "Không thể cập nhật. Vui lòng thử lại.";
            await LoadParentCategoriesAsync(token);
            return Page();
        }

        private async Task LoadParentCategoriesAsync(string? token)
        {
            var client = CreateAuthenticatedClient(token ?? "");
            var response = await client.GetAsync("api/admin/category?pageNumber=1&pageSize=1000");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MacVilla_Web.Models.PagedResponse<Category>>();
                ParentCategories = result?.Data?.Where(c => c.CategoryId != Id && !c.ParentCategoryId.HasValue).ToList() ?? new();
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
