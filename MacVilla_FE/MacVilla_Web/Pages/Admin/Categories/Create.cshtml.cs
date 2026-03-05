using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Categories;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public CreateModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty]
    public CreateCategoryRequest CreateRequest { get; set; } = new();

    public List<Category>? ParentCategories { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadParentCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(CreateRequest.CategoryName))
        {
            ModelState.AddModelError("CreateRequest.CategoryName", "Tên danh mục không được để trống.");
            await LoadParentCategoriesAsync();
            return Page();
        }

        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/admin/Category", CreateRequest);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Thêm danh mục thành công.";
            return RedirectToPage("Index");
        }

        // Cố gắng đọc thông báo lỗi chi tiết từ API (ví dụ: tên danh mục trùng)
        var error = await response.Content.ReadFromJsonAsync<SimpleErrorResponse>();
        ErrorMessage = !string.IsNullOrEmpty(error?.Message)
            ? error!.Message
            : "Không thể tạo danh mục. Vui lòng thử lại.";
        await LoadParentCategoriesAsync();
        return Page();
    }

    private async Task LoadParentCategoriesAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync("api/admin/Category?pageNumber=1&pageSize=1000");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResponse<Category>>();
            // Chỉ lấy các danh mục gốc (không có danh mục cha) để hiển thị trong dropdown "Danh mục cha"
            ParentCategories = result?.Data?
                .Where(c => !c.ParentCategoryId.HasValue)
                .ToList()
                ?? new List<Category>();
        }
        else
        {
            ParentCategories = new List<Category>();
        }
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _clientFactory.CreateClient("MacVillaAPI");
        var token = HttpContext.Session.GetString("JWToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    private class SimpleErrorResponse
    {
        public string? Message { get; set; }
    }
}
