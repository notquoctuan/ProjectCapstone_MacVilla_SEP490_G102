using System.Net.Http.Headers;
using MacVilla_Web.Models;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty(SupportsGet = true)]
        public CategorySearchRequest SearchRequest { get; set; } = new();

        public PagedResponse<Category>? PagedResult { get; set; }
        public string? Message { get; set; }
        public Dictionary<long, string> CategoryNameLookup { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var query = BuildQueryString();
            var response = await client.GetAsync($"api/admin/category?{query}");

            PagedResult = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<PagedResponse<Category>>()
                : new PagedResponse<Category>();

            await LoadCategoryLookupAsync(client);
            Message = TempData["Message"]?.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostActivateAsync(long id)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PatchAsync($"api/admin/category/{id}/activate", null);
            TempData["Message"] = "Đã kích hoạt danh mục.";
            return RedirectToPage(new { SearchRequest.Name, SearchRequest.IsActive, SearchRequest.PageNumber, SearchRequest.PageSize });
        }

        public async Task<IActionResult> OnPostDeactivateAsync(long id)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PatchAsync($"api/admin/category/{id}/deactivate", null);
            TempData["Message"] = "Đã vô hiệu hóa danh mục.";
            return RedirectToPage(new { SearchRequest.Name, SearchRequest.IsActive, SearchRequest.PageNumber, SearchRequest.PageSize });
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.DeleteAsync($"api/admin/category/{id}");
            TempData["Message"] = "Đã xóa danh mục.";
            return RedirectToPage(new { SearchRequest.Name, SearchRequest.IsActive, SearchRequest.PageNumber, SearchRequest.PageSize });
        }

        public Dictionary<string, string> GetPageRouteData(int pageNumber)
        {
            var data = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(SearchRequest.Name)) data["SearchRequest.Name"] = SearchRequest.Name;
            if (SearchRequest.IsActive.HasValue) data["SearchRequest.IsActive"] = SearchRequest.IsActive.Value.ToString();
            data["SearchRequest.PageNumber"] = pageNumber.ToString();
            data["SearchRequest.PageSize"] = SearchRequest.PageSize.ToString();
            return data;
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private string BuildQueryString()
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(SearchRequest.Name)) parts.Add($"name={Uri.EscapeDataString(SearchRequest.Name)}");
            if (SearchRequest.IsActive.HasValue) parts.Add($"isActive={SearchRequest.IsActive.Value}");
            parts.Add($"pageNumber={SearchRequest.PageNumber}");
            parts.Add($"pageSize={SearchRequest.PageSize}");
            return string.Join("&", parts);
        }

        private async Task LoadCategoryLookupAsync(HttpClient client)
        {
            var response = await client.GetAsync("api/admin/category/getall");
            if (response.IsSuccessStatusCode)
            {
                var allCats = await response.Content.ReadFromJsonAsync<List<CategoryVM>>();
                CategoryNameLookup = allCats?
                    .GroupBy(c => c.CategoryId)
                    .ToDictionary(g => g.Key, g => g.First().Name)
                    ?? new Dictionary<long, string>();
            }
        }
    }

    private class SimpleErrorResponse
    {
        public string? Message { get; set; }
    }
}
