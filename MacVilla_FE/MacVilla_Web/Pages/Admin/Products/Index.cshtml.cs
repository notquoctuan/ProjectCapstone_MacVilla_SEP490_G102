using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Bọc dữ liệu vào PagedResponse để lấy TotalPages, TotalCount
        public PagedResponse<ProductAdminVM> PagedProducts { get; set; } = new();

        // Bind dữ liệu Filter từ URL (Query String)
        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CategoryName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");

            // QUAN TRỌNG: Phải có "/filter" và truyền đúng các tham số
            var url = $"api/admin/products/filter?Name={Name}&CategoryName={CategoryName}&PageNumber={PageNumber}&PageSize=10";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Bây giờ API đã trả về đúng cục PagedResponse { data: [], totalCount: ... }
                var result = await response.Content.ReadFromJsonAsync<PagedResponse<ProductAdminVM>>();
                if (result != null)
                {
                    // Nếu bạn dùng PagedProducts ở FE, hãy gán vào nó
                    PagedProducts = result;
                }
            }
        }
    }
}   