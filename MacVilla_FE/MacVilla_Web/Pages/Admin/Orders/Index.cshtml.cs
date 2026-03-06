using System.Net.Http.Headers;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty(SupportsGet = true)]
        public OrderSearchRequest SearchRequest { get; set; } = new();

        public MacVilla_Web.Models.PagedResponse<OrderListResponse>? PagedResult { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var query = BuildQueryString();
            var response = await client.GetAsync($"api/admin/order?{query}");

            PagedResult = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<MacVilla_Web.Models.PagedResponse<OrderListResponse>>()
                : new MacVilla_Web.Models.PagedResponse<OrderListResponse>();

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(long id, string? reason)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PostAsJsonAsync($"api/admin/order/{id}/cancel", new { Reason = reason });
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(long id, string status, string? notes)
        {
            var client = CreateAuthenticatedClient(GetToken()!);
            await client.PutAsJsonAsync($"api/admin/order/{id}/status", new { Status = status, Notes = notes });
            return RedirectToPage();
        }

        public Dictionary<string, string> GetPageRouteData(int pageNumber)
        {
            var data = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(SearchRequest.Status)) data["SearchRequest.Status"] = SearchRequest.Status;
            if (SearchRequest.UserId.HasValue) data["SearchRequest.UserId"] = SearchRequest.UserId.Value.ToString();
            if (SearchRequest.StartDate.HasValue) data["SearchRequest.StartDate"] = SearchRequest.StartDate.Value.ToString("yyyy-MM-dd");
            if (SearchRequest.EndDate.HasValue) data["SearchRequest.EndDate"] = SearchRequest.EndDate.Value.ToString("yyyy-MM-dd");
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
            if (!string.IsNullOrEmpty(SearchRequest.Status)) parts.Add($"status={Uri.EscapeDataString(SearchRequest.Status)}");
            if (SearchRequest.UserId.HasValue) parts.Add($"userId={SearchRequest.UserId.Value}");
            if (SearchRequest.StartDate.HasValue) parts.Add($"startDate={SearchRequest.StartDate.Value:O}");
            if (SearchRequest.EndDate.HasValue) parts.Add($"endDate={SearchRequest.EndDate.Value:O}");
            parts.Add($"pageNumber={SearchRequest.PageNumber}");
            parts.Add($"pageSize={SearchRequest.PageSize}");
            return string.Join("&", parts);
        }
    }
}
