using MacVilla_Web.Models;
using System.Net.Http.Json;

namespace MacVilla_Web.Services
{
    public class CartApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<CartDto?> GetCartAsync()
        {
            try
            {
                var client = GetClient();
                var response = await client.GetAsync("api/cart");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CartDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<CartDto?> AddToCartAsync(long productId, int quantity = 1)
        {
            try
            {
                var client = GetClient();
                var request = new AddToCartRequest { ProductId = productId, Quantity = quantity };
                var response = await client.PostAsJsonAsync("api/cart/items", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CartDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<CartDto?> UpdateCartItemAsync(long cartItemId, int quantity)
        {
            try
            {
                var client = GetClient();
                var request = new UpdateCartItemRequest { CartItemId = cartItemId, Quantity = quantity };
                var response = await client.PutAsJsonAsync("api/cart/items", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CartDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<CartDto?> DeleteCartItemAsync(long cartItemId)
        {
            try
            {
                var client = GetClient();
                var response = await client.DeleteAsync($"api/cart/items/{cartItemId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CartDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
