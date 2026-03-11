using MacVilla_Web.Models;
using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly ProductApiService _productService;

        public EditModel(ProductApiService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public long Id { get; set; }

        [BindProperty]
        public string Name { get; set; } = "";

        [BindProperty]
        public decimal Price { get; set; }

        [BindProperty]
        public long CategoryId { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public string Status { get; set; } = "Pending";

        [BindProperty]
        public List<IFormFile>? NewImageFiles { get; set; }

        [BindProperty]
        public List<long>? DeleteImageIds { get; set; }

        [BindProperty]
        public long? MainImageId { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public List<ProductImageResponse> ExistingImages { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Id = id;

            var product = await _productService.GetProductDetailAsync(id);

            if (product == null)
                return RedirectToPage("./Index");

            Name = product.Name;
            Price = product.Price;
            CategoryId = product.CategoryId ?? 0;
            Description = product.Description;
            Status = product.Status;

            ExistingImages = product.Images;

            await LoadCategoriesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (success, message) = await _productService.UpdateProductAsync(
                Id,
                Name,
                Price,
                CategoryId,
                Description,
                Status,
                NewImageFiles,
                DeleteImageIds,
                MainImageId
            );

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToPage("./Index");
            }

            ErrorMessage = message;

            var product = await _productService.GetProductDetailAsync(Id);
            ExistingImages = product?.Images ?? new();

            await LoadCategoriesAsync();

            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _productService.GetCategoriesAsync();

            CategoryOptions = FlattenCategories(categories, 0);
        }

        private static List<SelectListItem> FlattenCategories(
            List<CategoryTreeResponse> items,
            int depth)
        {
            var list = new List<SelectListItem>();

            foreach (var item in items)
            {
                list.Add(new SelectListItem(
                    (depth > 0 ? new string('─', depth) + " " : "") + item.CategoryName,
                    item.CategoryId.ToString()));

                list.AddRange(
                    FlattenCategories(item.Children, depth + 1)
                );
            }

            return list;
        }
    }
}