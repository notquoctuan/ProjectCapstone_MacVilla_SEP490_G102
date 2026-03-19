using MacVilla_Web.Models;
using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductApiService _productApi;
        public DetailsModel(ProductApiService productApi) => _productApi = productApi;

        public ProductAdminVM Product { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var result = await _productApi.GetProductByIdAsync(id);
            if (result == null) return NotFound();

            Product = result;
            return Page();
        }
    }
}
