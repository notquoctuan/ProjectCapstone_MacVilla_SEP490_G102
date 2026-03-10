using MacVilla_Web.Models;
using System.Net.Http.Headers;

namespace MacVilla_Web.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient Client => _httpClient;

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<ProductAdminVM?> GetProductByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductAdminVM>($"api/admin/products/{id}");
            }
            catch { return null; }
        }
    }
}
