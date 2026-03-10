using MacVilla_Web.Models;
using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class DetailModel : PageModel
    {
        private readonly ProductApiService _productService;

        public DetailModel(ProductApiService productService)
            => _productService = productService;

        [BindProperty(SupportsGet = true)] public long Id { get; set; }

        // Dùng ProductDetailResponse đúng theo BE
        public ProductDetailResponse? Product { get; set; }

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Product = await _productService.GetProductDetailAsync(Id);
            if (Product is null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(string status)
        {
            var (success, message) = await _productService.ChangeStatusAsync(Id, status);
            if (success) SuccessMessage = message;
            else ErrorMessage = message;
            return RedirectToPage(new { id = Id });
        }
    }
}