using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public ProductCreateRequest Product { get; set; } = new();

        // Để hiển thị danh sách tên danh mục
        public List<CategoryVM> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");
            // Gọi API lấy danh mục (Dùng hàm GetAll của bạn)
            Categories = await client.GetFromJsonAsync<List<CategoryVM>>("api/admin/category/getall") ?? new();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(Product.Name), "Name");
            content.Add(new StringContent(Product.Price.ToString()), "Price");
            content.Add(new StringContent(Product.CategoryId.ToString()), "CategoryId");
            content.Add(new StringContent(Product.Description ?? ""), "Description");
            content.Add(new StringContent(Product.Status), "Status");

            if (Product.ImageFiles != null)
            {
                foreach (var file in Product.ImageFiles)
                {
                    var stream = new StreamContent(file.OpenReadStream());
                    content.Add(stream, "ImageFiles", file.FileName);
                }
            }

            var response = await client.PostAsync("api/admin/products", content);
            if (response.IsSuccessStatusCode) return RedirectToPage("/Admin/Products/Index");

            return Page();
        }
    }
}