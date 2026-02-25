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

        public List<CategoryVM> Categories { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");
            // 1. Lấy toàn bộ danh sách giống hệt bên Create
            var allCats = await client.GetFromJsonAsync<List<CategoryVM>>("api/admin/category/getall") ?? new();

            // 2. CHỈ LẤY CHA: Lọc những thằng không có ParentCategoryId (tức là null)
            Categories = allCats.Where(x => x.ParentCategoryId == null).ToList();

            // 3. Giữ nguyên logic lấy sản phẩm của bạn
            // QUAN TRỌNG: Phải có "/filter" và truyền đúng các tham số
            //var url = $"api/admin/products/filter?Name={Name}&CategoryName={CategoryName}&PageNumber={PageNumber}&PageSize=10";
            var url = $"api/admin/products/filter?Name={Name}&CategoryName={CategoryName}&SortOrder={SortOrder}&PageNumber={PageNumber}&PageSize=10";

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