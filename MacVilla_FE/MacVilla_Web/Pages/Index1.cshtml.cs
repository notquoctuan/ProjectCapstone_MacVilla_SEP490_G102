using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public List<dynamic> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient("MacVillaAPI");

        var response = await client.GetAsync("api/Product");

        if (response.IsSuccessStatusCode)
        {
            Products = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        }
    }
}