using MacVilla_Web.Models;
using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.HomePage;

public class CartModel : PageModel
{
    private readonly CartApiService _cartService;

    public CartModel(CartApiService cartService)
    {
        _cartService = cartService;
    }

    public CartViewModel ViewModel { get; set; } = new();

    public async Task OnGetAsync()
    {
        var cart = await _cartService.GetCartAsync();
        ViewModel.Cart = cart;
    }
}

