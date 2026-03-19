using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Orders;

public class DetailModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public DetailModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    public OrderDetailResponse? Order { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync($"api/admin/Order/{Id}");
        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        Order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>();
        if (Order == null)
        {
            return NotFound();
        }

        return Page();
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

