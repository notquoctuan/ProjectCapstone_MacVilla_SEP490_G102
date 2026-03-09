using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public ProductCreateRequest Product { get; set; } = new();
        public List<CategoryVM> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");
            var client = CreateAuthenticatedClient(token);
            Categories = await client.GetFromJsonAsync<List<CategoryVM>>("api/admin/category/getall") ?? new();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");
            var client = CreateAuthenticatedClient(token);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(Product.Name ?? ""), "Name");
            content.Add(new StringContent(Product.Price.ToString()), "Price");
            content.Add(new StringContent(Product.CategoryId.ToString()), "CategoryId");
            content.Add(new StringContent(Product.Description ?? ""), "Description");
            content.Add(new StringContent(Product.Status ?? "Pending"), "Status");

            if (Product.ImageFiles != null)
                foreach (var file in Product.ImageFiles)
                    content.Add(new StreamContent(file.OpenReadStream()), "ImageFiles", file.FileName);

            var response = await client.PostAsync("api/admin/products", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToPage("/Admin/Products/Index");
            }

            var errorResult = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Lỗi: {errorResult}");
            Categories = await client.GetFromJsonAsync<List<CategoryVM>>("api/admin/category/getall") ?? new();
            return Page();
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _httpClientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
