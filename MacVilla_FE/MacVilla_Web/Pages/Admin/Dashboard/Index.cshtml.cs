using System.Net.Http.Headers;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public List<OrderListResponse> RecentOrders { get; set; } = new();

        public IndexModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);

            var prodRes = await client.GetAsync("api/admin/products?PageNumber=1&PageSize=1");
            if (prodRes.IsSuccessStatusCode)
            {
                var paged = await prodRes.Content.ReadFromJsonAsync<Models.PagedResponse<Models.ProductAdminVM>>();
                TotalProducts = paged?.TotalCount ?? 0;
            }

            var orderRes = await client.GetAsync("api/admin/order?PageNumber=1&PageSize=5");
            if (orderRes.IsSuccessStatusCode)
            {
                var paged = await orderRes.Content.ReadFromJsonAsync<Models.PagedResponse<OrderListResponse>>();
                TotalOrders = paged?.TotalCount ?? 0;
                RecentOrders = paged?.Data?.ToList() ?? new();
            }

            var userRes = await client.GetAsync("api/admin/users?PageNumber=1&PageSize=1");
            if (userRes.IsSuccessStatusCode)
            {
                var paged = await userRes.Content.ReadFromJsonAsync<Models.PagedResponse<UserListVM>>();
                TotalUsers = paged?.TotalCount ?? 0;
            }

            return Page();
        }

        private string? GetToken()
            => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }

    public class UserListVM
    {
        public long UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }
}
