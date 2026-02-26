using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public EditModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty]
        public ProductUpdateVM UpdateData { get; set; } = new();

        public List<CategoryVM> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");

            // 1. Lấy thông tin sản phẩm
            var productRes = await client.GetAsync($"api/admin/products/{id}");
            var categoryRes = await client.GetAsync("api/admin/category/getall");

            if (productRes.IsSuccessStatusCode && categoryRes.IsSuccessStatusCode)
            {
                UpdateData = await productRes.Content.ReadFromJsonAsync<ProductUpdateVM>() ?? new();
                Categories = await categoryRes.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();
                return Page();
            }

            return RedirectToPage("/Admin/Products/Index");
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? NewImage)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(UpdateData.ProductId.ToString()), "ProductId");
            content.Add(new StringContent(UpdateData.Name ?? ""), "Name");
            content.Add(new StringContent(UpdateData.Price.ToString()), "Price");
            content.Add(new StringContent(UpdateData.CategoryId.ToString()), "CategoryId"); 
            content.Add(new StringContent(UpdateData.Status ?? "Pending"), "Status");
            content.Add(new StringContent(UpdateData.Description ?? ""), "Description");

            if (NewImage != null)
            {
                var fileContent = new StreamContent(NewImage.OpenReadStream());
                content.Add(fileContent, "NewImageFiles", NewImage.FileName);
            }

            var response = await client.PutAsync($"api/admin/products/update/{UpdateData.ProductId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToPage("/Admin/Products/Index");
            }

            var errorResult = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Lỗi từ hệ thống: {errorResult}");

            var categoryRes = await client.GetAsync("api/category/getall");
            Categories = await categoryRes.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();

            return Page();
        }
    }
}