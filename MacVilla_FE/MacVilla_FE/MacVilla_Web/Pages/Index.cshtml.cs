using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace MacVilla_Web.Pages
{
    public class HomepagePagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;

        public List<BannerVM> Banners { get; set; } = new();
        public List<CategoryTreeVM> CategoryTree { get; set; } = new();
        public List<ProductAdminVM> FeaturedProducts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public List<ProductAdminVM> SearchResults { get; set; } = new();
        public bool HasSearched { get; set; } = false;
        public string? SearchError { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");

            // Load banners
            try
            {
                var banners = await client.GetFromJsonAsync<List<BannerVM>>("api/home/banners");
                if (banners != null)
                    Banners = banners.OrderBy(b => b.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load banners for homepage");
            }

            // Load category tree
            try
            {
                var tree = await client.GetFromJsonAsync<List<CategoryTreeVM>>("api/home/categories");
                if (tree != null)
                    CategoryTree = tree;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load categories for homepage");
            }

            // Load featured products (top 8)
            try
            {
                var products = await client.GetFromJsonAsync<List<ProductAdminVM>>("api/home/featured-products");
                if (products != null)
                    FeaturedProducts = products;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load featured products for homepage");
            }

            // Handle search
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var trimmed = SearchQuery.Trim();
                if (trimmed.Length > 100)
                {
                    SearchError = "Từ khóa tìm kiếm không được vượt quá 100 ký tự.";
                    HasSearched = true;
                    return;
                }

                HasSearched = true;
                try
                {
                    var searchUrl = $"api/home/search?Keyword={Uri.EscapeDataString(trimmed)}&PageNumber=1&PageSize=20";
                    var response = await client.GetAsync(searchUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<HomepagePagedResponse<ProductAdminVM>>();
                        if (result?.Data != null)
                            SearchResults = result.Data;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        SearchError = "Từ khóa tìm kiếm không hợp lệ.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Search failed for query: {Query}", trimmed);
                    SearchError = "Đã xảy ra lỗi khi tìm kiếm. Vui lòng thử lại.";
                }
            }
        }
    }
}
