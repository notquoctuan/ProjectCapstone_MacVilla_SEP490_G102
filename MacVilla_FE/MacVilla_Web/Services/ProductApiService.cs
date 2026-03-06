namespace MacVilla_Web.Services
{
    /// <summary>
    /// Typed HttpClient service for Product API calls.
    /// Registered in Program.cs via AddHttpClient<ProductApiService>.
    /// </summary>
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient Client => _httpClient;
    }
}
