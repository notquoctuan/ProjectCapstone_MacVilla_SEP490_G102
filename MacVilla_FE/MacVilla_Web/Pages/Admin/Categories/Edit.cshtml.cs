using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Categories;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public EditModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    [BindProperty]
    public UpdateCategoryRequest UpdateRequest { get; set; } = new();

    public List<Category>? ParentCategories { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync($"api/admin/Category/{Id}");
        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var category = await response.Content.ReadFromJsonAsync<Category>();
        if (category == null)
        {
            return NotFound();
        }

        UpdateRequest = new UpdateCategoryRequest
        {
            CategoryName = category.CategoryName,
            ParentCategoryId = category.ParentCategoryId
        };

        await LoadParentCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(UpdateRequest.CategoryName))
        {
            ModelState.AddModelError("UpdateRequest.CategoryName", "Tên danh mục không được để trống.");
            await LoadParentCategoriesAsync();
            return Page();
        }

        var client = CreateAuthenticatedClient();
        var response = await client.PutAsJsonAsync($"api/admin/Category/{Id}", UpdateRequest);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Cập nhật danh mục thành công.";
            return RedirectToPage("Index");
        }

        ErrorMessage = response.StatusCode == System.Net.HttpStatusCode.BadRequest
            ? "Dữ liệu không hợp lệ. Danh mục không thể trở thành danh mục cha của chính nó."
            : "Không thể cập nhật. Vui lòng thử lại.";
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
            // Chỉ hiển thị các danh mục gốc (không có danh mục cha) và khác danh mục hiện tại
            ParentCategories = result?.Data?
                .Where(c => c.CategoryId != Id && !c.ParentCategoryId.HasValue)
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
}
