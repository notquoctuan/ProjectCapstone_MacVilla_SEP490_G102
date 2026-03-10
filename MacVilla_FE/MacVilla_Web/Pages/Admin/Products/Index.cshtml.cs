using MacVilla_Web.Models;
using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductApiService _productService;

        public IndexModel(ProductApiService productService)
            => _productService = productService;

        public PagedResponse<ProductAdminResponse>? Products { get; set; }

        [BindProperty(SupportsGet = true)] public string? SearchName { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchCategory { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchStatus { get; set; }
        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; } = "newest";
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
        public const int PageSize = 10;

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _productService.GetProductsAsync(
                name: SearchName,
                categoryName: SearchCategory,
                status: SearchStatus,
                sortOrder: SortOrder,
                pageNumber: PageNumber,
                pageSize: PageSize);
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var (success, message) = await _productService.DeleteProductAsync(id);
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(long id, string status)
        {
            var (success, message) = await _productService.ChangeStatusAsync(id, status);
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            return RedirectToPage();
        }
    }
}