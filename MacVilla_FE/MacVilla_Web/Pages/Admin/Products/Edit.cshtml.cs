using MacVilla_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace MacVilla_Web.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public EditModel(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        [BindProperty]
        public ProductUpdateVM UpdateData { get; set; } = new();
        public List<CategoryVM> Categories { get; set; } = new();
        public List<ProductImageVM> Images { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");
            var client = CreateAuthenticatedClient(token);

            var productRes = await client.GetAsync($"api/admin/products/{id}");
            var categoryRes = await client.GetAsync("api/admin/category/getall");

            if (!productRes.IsSuccessStatusCode) return RedirectToPage("/Admin/Products/Index");

            var productDetail = await productRes.Content.ReadFromJsonAsync<ProductDetailVM>();
            if (productDetail != null)
            {
                UpdateData = new ProductUpdateVM
                {
                    ProductId = productDetail.ProductId,
                    Name = productDetail.Name,
                    Price = productDetail.Price,
                    Description = productDetail.Description,
                    Status = productDetail.Status,
                    CategoryId = productDetail.CategoryId ?? 0,
                    ImageUrl = productDetail.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? productDetail.Images?.FirstOrDefault()?.ImageUrl
                };
                Images = productDetail.Images ?? new();
            }

            if (categoryRes.IsSuccessStatusCode)
                Categories = await categoryRes.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile>? NewImageFiles, string? DeleteImageIds, long? MainImageId)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Auth/Login");
            var client = CreateAuthenticatedClient(token);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(UpdateData.Name ?? ""), "Name");
            content.Add(new StringContent(UpdateData.Price.ToString()), "Price");
            content.Add(new StringContent(UpdateData.CategoryId.ToString()), "CategoryId");
            content.Add(new StringContent(UpdateData.Status ?? "Pending"), "Status");
            content.Add(new StringContent(UpdateData.Description ?? ""), "Description");

            if (NewImageFiles != null)
                foreach (var file in NewImageFiles)
                    content.Add(new StreamContent(file.OpenReadStream()), "NewImageFiles", file.FileName);

            if (!string.IsNullOrEmpty(DeleteImageIds))
                foreach (var idStr in DeleteImageIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    content.Add(new StringContent(idStr.Trim()), "DeleteImageIds");

            if (MainImageId.HasValue)
                content.Add(new StringContent(MainImageId.Value.ToString()), "MainImageId");

            var response = await client.PutAsync($"api/admin/products/{UpdateData.ProductId}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToPage("/Admin/Products/Index");
            }

            ErrorMessage = $"Lỗi: {await response.Content.ReadAsStringAsync()}";
            var categoryRes = await client.GetAsync("api/admin/category/getall");
            Categories = await categoryRes.Content.ReadFromJsonAsync<List<CategoryVM>>() ?? new();
            return Page();
        }

        private string? GetToken() => Request.Cookies["jwt"] ?? HttpContext.Session.GetString("JWToken");

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _clientFactory.CreateClient("MacVillaAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
