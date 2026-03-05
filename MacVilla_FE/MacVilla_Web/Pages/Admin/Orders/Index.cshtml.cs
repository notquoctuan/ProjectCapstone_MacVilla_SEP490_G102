using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Orders;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty(SupportsGet = true)]
    public OrderSearchRequest SearchRequest { get; set; } = new();

    public PagedResponse<OrderListResponse>? PagedResult { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = CreateAuthenticatedClient();
        var query = BuildQueryString();
        var response = await client.GetAsync($"api/admin/Order?{query}");

        if (response.IsSuccessStatusCode)
        {
            PagedResult = await response.Content.ReadFromJsonAsync<PagedResponse<OrderListResponse>>();
        }
        else
        {
            PagedResult = new PagedResponse<OrderListResponse>();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(long id, string? reason)
    {
        var client = CreateAuthenticatedClient();
        var request = new CancelOrderRequest { Reason = reason };
        var response = await client.PostAsJsonAsync($"api/admin/Order/{id}/cancel", request);
        return RedirectToPage(new
        {
            SearchRequest.Status,
            SearchRequest.UserId,
            SearchRequest.StartDate,
            SearchRequest.EndDate,
            SearchRequest.PageNumber,
            SearchRequest.PageSize
        });
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(long id, string status, string? notes)
    {
        var client = CreateAuthenticatedClient();
        var request = new UpdateOrderStatusRequest { Status = status, Notes = notes };
        var response = await client.PutAsJsonAsync($"api/admin/Order/{id}/status", request);
        return RedirectToPage(new
        {
            SearchRequest.Status,
            SearchRequest.UserId,
            SearchRequest.StartDate,
            SearchRequest.EndDate,
            SearchRequest.PageNumber,
            SearchRequest.PageSize
        });
    }

    public Dictionary<string, string> GetPageRouteData(int pageNumber)
    {
        var data = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(SearchRequest.Status))
            data["SearchRequest.Status"] = SearchRequest.Status;
        if (SearchRequest.UserId.HasValue)
            data["SearchRequest.UserId"] = SearchRequest.UserId.Value.ToString();
        if (SearchRequest.StartDate.HasValue)
            data["SearchRequest.StartDate"] = SearchRequest.StartDate.Value.ToString("yyyy-MM-dd");
        if (SearchRequest.EndDate.HasValue)
            data["SearchRequest.EndDate"] = SearchRequest.EndDate.Value.ToString("yyyy-MM-dd");
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
        if (!string.IsNullOrEmpty(SearchRequest.Status))
            parts.Add($"status={Uri.EscapeDataString(SearchRequest.Status)}");
        if (SearchRequest.UserId.HasValue)
            parts.Add($"userId={SearchRequest.UserId.Value}");
        if (SearchRequest.StartDate.HasValue)
            parts.Add($"startDate={SearchRequest.StartDate.Value:O}");
        if (SearchRequest.EndDate.HasValue)
            parts.Add($"endDate={SearchRequest.EndDate.Value:O}");
        parts.Add($"pageNumber={SearchRequest.PageNumber}");
        parts.Add($"pageSize={SearchRequest.PageSize}");
        return string.Join("&", parts);
    }
}

