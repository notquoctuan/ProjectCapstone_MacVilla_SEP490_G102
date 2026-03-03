using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Categories;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty(SupportsGet = true)]
    public CategorySearchRequest SearchRequest { get; set; } = new();

    public PagedResponse<Category>? PagedResult { get; set; }
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = CreateAuthenticatedClient();
        var query = BuildQueryString();
        var response = await client.GetAsync($"api/admin/Category?{query}");

        if (response.IsSuccessStatusCode)
        {
            PagedResult = await response.Content.ReadFromJsonAsync<PagedResponse<Category>>();
        }
        else
        {
            PagedResult = new PagedResponse<Category>();
        }

        Message = TempData["Message"]?.ToString();
        return Page();
    }

    public async Task<IActionResult> OnPostActivateAsync(long id)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync($"api/admin/Category/{id}/activate", null);
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Đã kích hoạt danh mục thành công.";
        }
        else
        {
            TempData["Message"] = "Không thể kích hoạt. Vui lòng thử lại.";
        }
        return RedirectToPage(new { SearchRequest.Name, SearchRequest.IsActive, SearchRequest.PageNumber, SearchRequest.PageSize });
    }

    public async Task<IActionResult> OnPostDeactivateAsync(long id)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync($"api/admin/Category/{id}/deactivate", null);
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Đã ngừng hoạt động danh mục thành công.";
        }
        else
        {
            TempData["Message"] = "Không thể ngừng hoạt động. Vui lòng thử lại.";
        }
        return RedirectToPage(new { SearchRequest.Name, SearchRequest.IsActive, SearchRequest.PageNumber, SearchRequest.PageSize });
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.DeleteAsync($"api/admin/Category/{id}");
        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "Đã xóa danh mục thành công.";
        }
        else
        {
            TempData["Message"] = "Không thể xóa. Vui lòng thử lại.";
        }
        return RedirectToPage(new { SearchRequest.Name, SearchRequest.IsActive, SearchRequest.PageNumber, SearchRequest.PageSize });
    }

    public Dictionary<string, string> GetPageRouteData(int pageNumber)
    {
        var data = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(SearchRequest.Name))
            data["SearchRequest.Name"] = SearchRequest.Name;
        if (SearchRequest.IsActive.HasValue)
            data["SearchRequest.IsActive"] = SearchRequest.IsActive.Value.ToString();
        data["SearchRequest.PageNumber"] = pageNumber.ToString();
        data["SearchRequest.PageSize"] = SearchRequest.PageSize.ToString();
        return data;
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

    private string BuildQueryString()
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(SearchRequest.Name))
            parts.Add($"name={Uri.EscapeDataString(SearchRequest.Name)}");
        if (SearchRequest.IsActive.HasValue)
            parts.Add($"isActive={SearchRequest.IsActive.Value}");
        parts.Add($"pageNumber={SearchRequest.PageNumber}");
        parts.Add($"pageSize={SearchRequest.PageSize}");
        return string.Join("&", parts);
    }
}
