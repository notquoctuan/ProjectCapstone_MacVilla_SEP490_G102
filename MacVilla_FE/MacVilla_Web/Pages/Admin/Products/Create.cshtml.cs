using System.ComponentModel.DataAnnotations;
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

        [BindProperty]
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Tên sản phẩm phải từ 2 đến 200 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(1000, 999999999,
            ErrorMessage = "Giá phải từ 1,000 đến 999,999,999.")]
        public decimal Price { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Range(1, long.MaxValue,
            ErrorMessage = "Danh mục không hợp lệ.")]
        public long CategoryId { get; set; }

        [BindProperty]
        [StringLength(5000,
            ErrorMessage = "Mô tả không được vượt quá 5000 ký tự.")]
        public string? Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Enable|Disable|Pending)$",
            ErrorMessage = "Trạng thái chỉ được là Enable, Disable hoặc Pending.")]
        public string Status { get; set; } = "Pending";

        [BindProperty]
        public List<IFormFile>? ImageFiles { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

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