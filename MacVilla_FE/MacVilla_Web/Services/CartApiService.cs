using System.Net.Http.Headers;
using System.Net.Http.Json;
using MacVilla_Web.DTOs;

namespace MacVilla_Web.Services;

public class CartApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AttachToken()
    {
        var http = _httpContextAccessor.HttpContext;
        var token =
            http?.Session.GetString("JwtToken") ??
            http?.Session.GetString("JWToken") ??
            http?.Request.Cookies["jwt"];

        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<CartDto?> GetCartAsync()
    {
        AttachToken();
        var resp = await _httpClient.GetAsync("api/cart");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<CartDto>();
    }

    public async Task<CartDto?> AddItemAsync(long productId, int quantity)
    {
        AttachToken();
        var body = new AddToCartRequest(productId, quantity);
        var resp = await _httpClient.PostAsJsonAsync("api/cart/items", body);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<CartDto>();
    }

    public async Task<CartDto?> UpdateItemAsync(long cartItemId, int quantity)
    {
        AttachToken();
        var body = new UpdateCartItemRequest(cartItemId, quantity);
        var resp = await _httpClient.PutAsJsonAsync("api/cart/items", body);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<CartDto>();
    }

    public async Task<CartDto?> RemoveItemAsync(long cartItemId)
    {
        AttachToken();
        var resp = await _httpClient.DeleteAsync($"api/cart/items/{cartItemId}");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<CartDto>();
    }

    public async Task ClearAsync()
    {
        AttachToken();
        await _httpClient.DeleteAsync("api/cart");
    }
}

