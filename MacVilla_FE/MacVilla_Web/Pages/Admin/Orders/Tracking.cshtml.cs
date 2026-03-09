using System.Net.Http.Headers;
using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Orders
{
    public class TrackingModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public TrackingModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }
        public OrderTrackingResponse? Tracking { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");

            var client = CreateAuthenticatedClient(token);
            var response = await client.GetAsync($"api/admin/order/{Id}/tracking");
            if (!response.IsSuccessStatusCode) return NotFound();

            Tracking = await response.Content.ReadFromJsonAsync<OrderTrackingResponse>();
            if (Tracking == null) return NotFound();
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
