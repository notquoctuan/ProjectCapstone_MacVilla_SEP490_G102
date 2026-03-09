using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PagedResponse<ProductAdminVM> PagedProducts { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? Name { get; set; }
        [BindProperty(SupportsGet = true)] public string? CategoryName { get; set; }
        [BindProperty(SupportsGet = true)] public string? SortOrder { get; set; }
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

        public List<CategoryVM> Categories { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);

            var catRes = await client.GetAsync("api/admin/category/getall");
            if (catRes.IsSuccessStatusCode)
                Categories = await catRes.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();

            var queryParts = new List<string>();
            if (!string.IsNullOrEmpty(Name)) queryParts.Add($"Name={Uri.EscapeDataString(Name)}");
            if (!string.IsNullOrEmpty(CategoryName)) queryParts.Add($"CategoryName={Uri.EscapeDataString(CategoryName)}");
            if (!string.IsNullOrEmpty(SortOrder)) queryParts.Add($"SortOrder={Uri.EscapeDataString(SortOrder)}");
            queryParts.Add($"PageNumber={PageNumber}");
            queryParts.Add("PageSize=10");

            var url = "api/admin/products?" + string.Join("&", queryParts);
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResponse<ProductAdminVM>>();
                if (result != null) PagedProducts = result;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToPage("/Auth/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.DeleteAsync($"api/admin/products/{id}");
            return RedirectToPage(new { Name, CategoryName, SortOrder, PageNumber });
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(long id, string status)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PatchAsJsonAsync($"api/admin/products/{id}/status", new { Status = status });
            return RedirectToPage(new { Name, CategoryName, SortOrder, PageNumber });
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
