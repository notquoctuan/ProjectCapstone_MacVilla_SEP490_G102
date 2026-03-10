using MacVilla_Web.Models;
using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly ProductApiService _productService;

        public CreateModel(ProductApiService productService)
            => _productService = productService;

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        [BindProperty] public string Name { get; set; } = string.Empty;
        [BindProperty] public decimal Price { get; set; }
        [BindProperty] public long CategoryId { get; set; }
        [BindProperty] public string? Description { get; set; }
        [BindProperty] public string Status { get; set; } = "Pending";
        [BindProperty] public List<IFormFile>? ImageFiles { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
            => await LoadCategoriesAsync();

        public async Task<IActionResult> OnPostAsync()
        {
            var (success, message) = await _productService.CreateProductAsync(
                Name, Price, CategoryId, Description, Status, ImageFiles);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToPage("./Index");
            }

            ErrorMessage = message;
            await LoadCategoriesAsync();
            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _productService.GetCategoriesAsync();
            CategoryOptions = FlattenCategories(categories, 0);
        }

        private static List<SelectListItem> FlattenCategories(
            List<CategoryTreeResponse> items, int depth)
        {
            var list = new List<SelectListItem>();
            foreach (var item in items)
            {
                list.Add(new SelectListItem(
                    (depth > 0 ? new string('─', depth) + " " : "") + item.CategoryName,
                    item.CategoryId.ToString()));
                list.AddRange(FlattenCategories(item.Children, depth + 1));
            }
            return list;
        }
    }
}