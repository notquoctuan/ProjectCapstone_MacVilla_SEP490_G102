using MacVilla_Web.Models;

namespace MacVilla_Web.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;
        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7262/"); 
        }

        public async Task<List<ProductAdminVM>> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductAdminVM>>("api/admin/products");
        }
        // Thêm vào trong class ProductApiService
        public async Task<ProductAdminVM?> GetProductByIdAsync(long id)
        {
            try
            {
                // Gọi đến endpoint api/admin/products/{id} của Backend
                return await _httpClient.GetFromJsonAsync<ProductAdminVM>($"api/admin/products/{id}");
            }
            catch
            {
                return null;
            }
        }
        public async Task UpdateStatusAsync(long id, string status)
        {
            await _httpClient.PatchAsync($"api/admin/products/{id}/status?status={status}", null);
        }
    }
}
