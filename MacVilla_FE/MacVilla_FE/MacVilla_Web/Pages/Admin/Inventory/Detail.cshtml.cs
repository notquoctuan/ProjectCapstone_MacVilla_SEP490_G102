using System.Net.Http.Headers;
using MacVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Inventory;

public class DetailModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public DetailModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    public InventoryDetailResponse? Inventory { get; set; }

    [BindProperty]
    public UpdateInventoryRequest UpdateRequest { get; set; } = new();

    [BindProperty]
    public AdjustInventoryRequest AdjustRequest { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync($"api/admin/Inventory/{Id}");
        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        Inventory = await response.Content.ReadFromJsonAsync<InventoryDetailResponse>();
        if (Inventory == null)
        {
            return NotFound();
        }

        UpdateRequest.Quantity = Inventory.Quantity ?? 0;
        UpdateRequest.WarehouseLocation = Inventory.WarehouseLocation;

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PutAsJsonAsync($"api/admin/Inventory/product/{Id}", UpdateRequest);

        if (!response.IsSuccessStatusCode)
        {
            // TODO: you can add TempData message if needed
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostAdjustAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync($"api/admin/Inventory/product/{Id}/adjust", AdjustRequest);

        if (!response.IsSuccessStatusCode)
        {
            // TODO: you can add TempData message if needed
        }

        return RedirectToPage(new { Id });
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

