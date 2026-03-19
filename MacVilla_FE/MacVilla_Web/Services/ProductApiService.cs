using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MacVilla_Web.Models;

namespace MacVilla_Web.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachToken()
        {
            var http = _httpContextAccessor.HttpContext;
            var token =
                http?.Session.GetString("JwtToken") ??
                http?.Session.GetString("JWToken") ??
                http?.Request.Cookies["jwt"];

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // GET: danh sách có lọc + phân trang
        public async Task<PagedResponse<ProductAdminResponse>?> GetProductsAsync(
            string? name = null, string? categoryName = null, string? status = null,
            string sortOrder = "newest", int pageNumber = 1, int pageSize = 10)
        {
            AttachToken();
            var q = new List<string>
            {
                $"pageNumber={pageNumber}", $"pageSize={pageSize}", $"sortOrder={sortOrder}"
            };
            if (!string.IsNullOrWhiteSpace(name)) q.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrWhiteSpace(categoryName)) q.Add($"categoryName={Uri.EscapeDataString(categoryName)}");
            if (!string.IsNullOrWhiteSpace(status)) q.Add($"status={status}");

            var resp = await _httpClient.GetAsync($"api/admin/products?{string.Join("&", q)}");
            if (!resp.IsSuccessStatusCode) return null;
            return Deserialize<PagedResponse<ProductAdminResponse>>(await resp.Content.ReadAsStringAsync());
        }

        // GET: chi tiết 1 sản phẩm (trả ProductDetailResponse với Images: List<ProductImageResponse>)
        public async Task<ProductDetailResponse?> GetProductDetailAsync(long id)
        {
            AttachToken();
            var resp = await _httpClient.GetAsync($"api/admin/products/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return Deserialize<ProductDetailResponse>(await resp.Content.ReadAsStringAsync());
        }

        // POST: tạo sản phẩm mới
        public async Task<(bool Success, string Message)> CreateProductAsync(
            string name, decimal price, long categoryId,
            string? description, string status, IList<IFormFile>? images)
        {
            AttachToken();
            using var form = BuildBaseForm(name, price, categoryId, description, status);
            if (images != null)
                foreach (var f in images)
                    form.Add(MakeStreamContent(f), "ImageFiles", f.FileName);

            return await ToResult(await _httpClient.PostAsync("api/admin/products", form),
                "Tạo sản phẩm thành công!");
        }

        // PUT: cập nhật sản phẩm
        public async Task<(bool Success, string Message)> UpdateProductAsync(
            long id, string name, decimal price, long categoryId,
            string? description, string status,
            IList<IFormFile>? newImages, List<long>? deleteImageIds, long? mainImageId)
        {
            AttachToken();
            using var form = BuildBaseForm(name, price, categoryId, description, status);

            if (mainImageId.HasValue)
                form.Add(new StringContent(mainImageId.Value.ToString()), "MainImageId");
            if (deleteImageIds != null)
                foreach (var imgId in deleteImageIds)
                    form.Add(new StringContent(imgId.ToString()), "DeleteImageIds");
            if (newImages != null)
                foreach (var f in newImages)
                    form.Add(MakeStreamContent(f), "NewImageFiles", f.FileName);

            return await ToResult(await _httpClient.PutAsync($"api/admin/products/{id}", form),
                "Cập nhật sản phẩm thành công!");
        }

        // PATCH: đổi trạng thái
        public async Task<(bool Success, string Message)> ChangeStatusAsync(long id, string status)
        {
            AttachToken();
            var body = new StringContent(
                JsonSerializer.Serialize(new { Status = status }), Encoding.UTF8, "application/json");
            return await ToResult(await _httpClient.PatchAsync($"api/admin/products/{id}/status", body),
                $"Đã chuyển trạng thái sang '{status}'.");
        }

        // DELETE: xóa sản phẩm
        public async Task<(bool Success, string Message)> DeleteProductAsync(long id)
        {
            AttachToken();
            return await ToResult(await _httpClient.DeleteAsync($"api/admin/products/{id}"),
                "Xóa sản phẩm thành công!");
        }

        // GET: danh mục (build cây từ danh sách phẳng /api/admin/category/getall)
        public async Task<List<CategoryTreeResponse>> GetCategoriesAsync()
        {
            AttachToken();
            var resp = await _httpClient.GetAsync("api/admin/category/getall");
            if (!resp.IsSuccessStatusCode) return new();

            var flat = Deserialize<List<CategoryVM>>(await resp.Content.ReadAsStringAsync()) ?? new();
            var lookup = flat.ToDictionary(c => c.CategoryId, c => new CategoryTreeResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = c.Name
            });

            foreach (var c in flat)
            {
                if (c.ParentCategoryId.HasValue && lookup.TryGetValue(c.ParentCategoryId.Value, out var parent))
                {
                    parent.Children.Add(lookup[c.CategoryId]);
                }
            }

            // chỉ trả về các root
            var rootIds = flat.Where(c => !c.ParentCategoryId.HasValue).Select(c => c.CategoryId).ToHashSet();
            return lookup.Values.Where(v => rootIds.Contains(v.CategoryId)).OrderBy(v => v.CategoryName).ToList();
        }

        // ── Helpers ──────────────────────────────────────────────────
        private static MultipartFormDataContent BuildBaseForm(
            string name, decimal price, long categoryId, string? description, string status)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(name), "Name");
            form.Add(new StringContent(price.ToString(CultureInfo.InvariantCulture)), "Price");
            form.Add(new StringContent(categoryId.ToString()), "CategoryId");
            form.Add(new StringContent(status), "Status");
            if (!string.IsNullOrWhiteSpace(description))
                form.Add(new StringContent(description), "Description");
            return form;
        }

        private static StreamContent MakeStreamContent(IFormFile file)
        {
            var sc = new StreamContent(file.OpenReadStream());
            sc.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            return sc;
        }

        private static async Task<(bool, string)> ToResult(HttpResponseMessage resp, string successMsg)
        {
            var body = await resp.Content.ReadAsStringAsync();
            return resp.IsSuccessStatusCode ? (true, successMsg) : (false, ExtractMessage(body));
        }

        private static string ExtractMessage(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("message", out var m))
                    return m.GetString() ?? "Lỗi không xác định.";
            }
            catch { }
            return "Lỗi không xác định.";
        }

        private static T? Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}