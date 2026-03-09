using System.Net.Http.Headers;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Orders
{
    public class DetailModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public DetailModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }
        public OrderDetailResponse? Order { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var response = await client.GetAsync($"api/admin/order/{Id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            Order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>();
            if (Order == null) return NotFound();
            return Page();
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
