using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Inventory;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty(SupportsGet = true)]
    public InventorySearchRequest SearchRequest { get; set; } = new();

    public PagedResponse<InventorySummaryResponse>? PagedResult { get; set; }
    public InventoryStatisticsResponse? Statistics { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = CreateAuthenticatedClient();

        // Danh sách tồn kho
        var query = BuildQueryString();
        var listResponse = await client.GetAsync($"api/admin/Inventory?{query}");
        if (listResponse.IsSuccessStatusCode)
        {
            PagedResult = await listResponse.Content.ReadFromJsonAsync<PagedResponse<InventorySummaryResponse>>();
        }
        else
        {
            PagedResult = new PagedResponse<InventorySummaryResponse>();
        }

        // Thống kê
        var statsResponse = await client.GetAsync("api/admin/Inventory/statistics");
        if (statsResponse.IsSuccessStatusCode)
        {
            Statistics = await statsResponse.Content.ReadFromJsonAsync<InventoryStatisticsResponse>();
        }

        return Page();
    }

    public Dictionary<string, string> GetPageRouteData(int pageNumber)
    {
        var data = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(SearchRequest.Keyword))
            data["SearchRequest.Keyword"] = SearchRequest.Keyword;
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
        if (!string.IsNullOrEmpty(SearchRequest.Keyword))
            parts.Add($"keyword={Uri.EscapeDataString(SearchRequest.Keyword)}");
        parts.Add($"pageNumber={SearchRequest.PageNumber}");
        parts.Add($"pageSize={SearchRequest.PageSize}");
        return string.Join("&", parts);
    }
}

