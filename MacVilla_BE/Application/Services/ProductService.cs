using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB
        private const int MaxImageCount = 10;

        public ProductService(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        // ────────────────────────────────────────────────────────────────
        // CREATE
        // ────────────────────────────────────────────────────────────────
        public async Task<ProductAdminResponse> CreateProductAsync(ProductCreateRequest request, string webRootPath)
        {
            var category = await _categoryRepo.GetByIdAsync(request.CategoryId)
                ?? throw new KeyNotFoundException($"Danh mục ID {request.CategoryId} không tồn tại.");

            if (await _productRepo.ExistsByNameAsync(request.Name, null))
                throw new InvalidOperationException($"Sản phẩm '{request.Name}' đã tồn tại.");

            var imageFiles = request.ImageFiles ?? new List<IFormFile>();
            ValidateImages(imageFiles);

            var product = new Product
            {
                Name = request.Name.Trim(),
                Price = request.Price,
                CategoryId = request.CategoryId,
                Description = request.Description?.Trim(),
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            if (imageFiles.Any())
                product.ProductImages = await SaveImagesAsync(imageFiles, webRootPath);

            var saved = await _productRepo.AddAsync(product);
            return MapToAdminResponse(saved, category.CategoryName);
        }

        // ────────────────────────────────────────────────────────────────
        // GET DETAIL
        // ────────────────────────────────────────────────────────────────
        public async Task<ProductDetailResponse?> GetProductDetailAsync(long id)
        {
            if (id <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ.");

            var product = await _productRepo.GetByIdDetailAsync(id);
            if (product == null) return null;

            return new ProductDetailResponse
            {
                ProductId = product.ProductId,
                Name = product.Name ?? "",
                Price = product.Price ?? 0,
                Description = product.Description,
                Status = product.Status ?? "Pending",
                CreatedAt = product.CreatedAt ?? DateTime.UtcNow,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName,
                Images = product.ProductImages
                    .OrderByDescending(i => i.IsMain == true)
                    .ThenBy(i => i.ImageId)
                    .Select(img => new ProductImageResponse
                    {
                        ImageId = img.ImageId,
                        ImageUrl = img.ImageUrl ?? "",
                        IsMain = img.IsMain == true
                    }).ToList()
            };
        }

        // ────────────────────────────────────────────────────────────────
        // GET PAGED
        // ────────────────────────────────────────────────────────────────
        public async Task<PagedResponse<ProductAdminResponse>> GetPagedProductsAsync(ProductSearchRequest request)
        {
            if (request.MinPrice.HasValue && request.MaxPrice.HasValue
                && request.MinPrice > request.MaxPrice)
                throw new ArgumentException("Giá tối thiểu không được lớn hơn giá tối đa.");

            var query = _productRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(p => p.Name != null && p.Name.Contains(request.Name.Trim()));

            if (!string.IsNullOrWhiteSpace(request.CategoryName))
                query = query.Where(p => p.Category != null
                    && p.Category.CategoryName != null
                    && p.Category.CategoryName.Contains(request.CategoryName.Trim()));

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(p => p.Status == request.Status);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            query = request.SortOrder switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "oldest" => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductAdminResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price ?? 0,
                    Description = p.Description,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                    Status = p.Status,
                    CreatedAt = p.CreatedAt ?? DateTime.UtcNow,
                    // Ưu tiên ảnh IsMain, fallback ảnh đầu tiên
                    ImageUrl = p.ProductImages
                        .Where(img => img.IsMain == true)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                        ?? p.ProductImages.Select(img => img.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();

            return new PagedResponse<ProductAdminResponse>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }

        // ────────────────────────────────────────────────────────────────
        // UPDATE — Giữ ảnh cũ, thêm/xóa ảnh linh hoạt
        // ────────────────────────────────────────────────────────────────
        public async Task UpdateProductAsync(long id, ProductUpdateRequest request, string webRootPath)
        {
            if (id <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ.");

            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

            var category = await _categoryRepo.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Danh mục ID {request.CategoryId} không tồn tại.");

            if (await _productRepo.ExistsByNameAsync(request.Name, excludeId: id))
                throw new InvalidOperationException($"Sản phẩm '{request.Name}' đã tồn tại.");

            // ── 1. Xóa ảnh theo yêu cầu ─────────────────────────────
            if (request.DeleteImageIds != null && request.DeleteImageIds.Any())
            {
                // Kiểm tra tất cả DeleteImageIds phải thuộc sản phẩm này
                var invalidIds = request.DeleteImageIds
                    .Where(imgId => !product.ProductImages.Any(i => i.ImageId == imgId))
                    .ToList();
                if (invalidIds.Any())
                    throw new ArgumentException($"ImageId không hợp lệ: {string.Join(", ", invalidIds)}.");

                foreach (var imgId in request.DeleteImageIds)
                {
                    var img = product.ProductImages.First(i => i.ImageId == imgId);
                    DeleteImageFile(img, webRootPath);
                    await _productRepo.DeleteImageAsync(img);
                }
            }

            // ── 2. Thêm ảnh mới ──────────────────────────────────────
            if (request.NewImageFiles != null && request.NewImageFiles.Any())
            {
                ValidateImages(request.NewImageFiles);

                // Kiểm tra tổng số ảnh sau khi thêm
                var remainCount = product.ProductImages.Count(i =>
                    request.DeleteImageIds == null || !request.DeleteImageIds.Contains(i.ImageId));
                if (remainCount + request.NewImageFiles.Count > MaxImageCount)
                    throw new ArgumentException($"Tổng số ảnh không được vượt quá {MaxImageCount}. Hiện tại còn {remainCount} ảnh.");

                var newImages = await SaveImagesAsync(request.NewImageFiles, webRootPath);

                // Nếu không còn ảnh nào → ảnh upload đầu tiên tự động là ảnh chính
                bool hasNoMain = !product.ProductImages.Any(i =>
                    i.IsMain == true &&
                    (request.DeleteImageIds == null || !request.DeleteImageIds.Contains(i.ImageId)));

                for (int i = 0; i < newImages.Count; i++)
                {
                    newImages[i].ProductId = product.ProductId;
                    if (hasNoMain && i == 0) newImages[i].IsMain = true;
                    await _productRepo.AddImageAsync(newImages[i]);
                }
            }

            // ── 3. Đổi ảnh chính ─────────────────────────────────────
            if (request.MainImageId.HasValue)
            {
                // Kiểm tra ảnh chính mới phải thuộc sản phẩm này và không nằm trong danh sách xóa
                var mainImg = product.ProductImages
                    .FirstOrDefault(i => i.ImageId == request.MainImageId.Value);

                if (mainImg == null)
                    throw new ArgumentException($"ImageId {request.MainImageId} không thuộc sản phẩm này.");

                if (request.DeleteImageIds != null && request.DeleteImageIds.Contains(request.MainImageId.Value))
                    throw new ArgumentException("Không thể đặt ảnh đang bị xóa làm ảnh chính.");

                // Bỏ IsMain tất cả ảnh cũ
                foreach (var img in product.ProductImages)
                    img.IsMain = false;

                mainImg.IsMain = true;
            }

            // ── 4. Cập nhật thông tin sản phẩm ───────────────────────
            product.Name = request.Name.Trim();
            product.Price = request.Price;
            product.Description = request.Description?.Trim();
            product.CategoryId = request.CategoryId;
            product.Status = request.Status;

            await _productRepo.UpdateAsync(product);
        }

        // ────────────────────────────────────────────────────────────────
        // THÊM ẢNH RIÊNG LẺ
        // ────────────────────────────────────────────────────────────────
        public async Task<List<ProductImageResponse>> AddImagesAsync(long productId, List<IFormFile> files, string webRootPath)
        {
            if (productId <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ.");

            var product = await _productRepo.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

            ValidateImages(files);

            if (product.ProductImages.Count + files.Count > MaxImageCount)
                throw new ArgumentException($"Tổng số ảnh không được vượt quá {MaxImageCount}. Hiện có {product.ProductImages.Count} ảnh.");

            bool isFirstEver = !product.ProductImages.Any();
            var newImages = await SaveImagesAsync(files, webRootPath);

            for (int i = 0; i < newImages.Count; i++)
            {
                newImages[i].ProductId = productId;
                if (isFirstEver && i == 0) newImages[i].IsMain = true;
                await _productRepo.AddImageAsync(newImages[i]);
            }

            await _productRepo.SaveAsync();

            return newImages.Select(img => new ProductImageResponse
            {
                ImageId = img.ImageId,
                ImageUrl = img.ImageUrl ?? "",
                IsMain = img.IsMain == true
            }).ToList();
        }

        // ────────────────────────────────────────────────────────────────
        // XÓA ẢNH RIÊNG LẺ
        // ────────────────────────────────────────────────────────────────
        public async Task DeleteImageAsync(long productId, long imageId, string webRootPath)
        {
            var product = await _productRepo.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

            var image = product.ProductImages.FirstOrDefault(i => i.ImageId == imageId)
                ?? throw new KeyNotFoundException("Không tìm thấy ảnh.");

            if (product.ProductImages.Count == 1)
                throw new InvalidOperationException("Sản phẩm phải có ít nhất 1 ảnh, không thể xóa ảnh duy nhất.");

            bool wasMain = image.IsMain == true;

            DeleteImageFile(image, webRootPath);
            await _productRepo.DeleteImageAsync(image);

            // Nếu xóa ảnh chính → tự động đặt ảnh đầu tiên còn lại làm ảnh chính
            if (wasMain)
            {
                var nextMain = product.ProductImages
                    .FirstOrDefault(i => i.ImageId != imageId);
                if (nextMain != null) nextMain.IsMain = true;
            }

            await _productRepo.SaveAsync();
        }

        // ────────────────────────────────────────────────────────────────
        // ĐẶT ẢNH CHÍNH
        // ────────────────────────────────────────────────────────────────
        public async Task SetMainImageAsync(long productId, long imageId)
        {
            var product = await _productRepo.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

            var target = product.ProductImages.FirstOrDefault(i => i.ImageId == imageId)
                ?? throw new KeyNotFoundException("Không tìm thấy ảnh hoặc ảnh không thuộc sản phẩm này.");

            foreach (var img in product.ProductImages)
                img.IsMain = (img.ImageId == imageId);

            await _productRepo.UpdateAsync(product);
        }

        // ────────────────────────────────────────────────────────────────
        // CHANGE STATUS
        // ────────────────────────────────────────────────────────────────
        public async Task ChangeStatusAsync(long id, string newStatus)
        {
            if (id <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ.");

            var validStatuses = new[] { "Enable", "Disable", "Pending" };
            if (!validStatuses.Contains(newStatus))
                throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận: Enable, Disable, Pending.");

            var updated = await _productRepo.UpdateStatusAsync(id, newStatus);
            if (!updated)
                throw new KeyNotFoundException("Không tìm thấy sản phẩm.");
        }

        // ────────────────────────────────────────────────────────────────
        // DELETE PRODUCT
        // ────────────────────────────────────────────────────────────────
        public async Task DeleteProductAsync(long id, string webRootPath)
        {
            if (id <= 0) throw new ArgumentException("ID sản phẩm không hợp lệ.");

            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

            foreach (var img in product.ProductImages)
                DeleteImageFile(img, webRootPath);

            await _productRepo.DeleteAsync(product);
        }

        // ────────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ────────────────────────────────────────────────────────────────
        private static void ValidateImages(IList<IFormFile> files)
        {
            if (files == null || !files.Any()) return;

            if (files.Count > MaxImageCount)
                throw new ArgumentException($"Chỉ được upload tối đa {MaxImageCount} ảnh một lần.");

            foreach (var file in files)
            {
                if (file.Length == 0)
                    throw new ArgumentException($"File '{file.FileName}' bị rỗng.");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                    throw new ArgumentException($"File '{file.FileName}' không hợp lệ. Chỉ chấp nhận: jpg, jpeg, png, webp.");

                if (file.Length > MaxImageSizeBytes)
                    throw new ArgumentException($"File '{file.FileName}' vượt quá 5MB.");
            }
        }

        private static async Task<List<ProductImage>> SaveImagesAsync(IList<IFormFile> files, string webRootPath)
        {
            var uploadPath = Path.Combine(webRootPath, "uploads", "products");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var images = new List<ProductImage>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                images.Add(new ProductImage
                {
                    ImageUrl = $"/uploads/products/{fileName}",
                    IsMain = false
                });
            }

            return images;
        }

        private static void DeleteImageFile(ProductImage image, string webRootPath)
        {
            if (string.IsNullOrEmpty(image.ImageUrl)) return;
            var fullPath = Path.Combine(webRootPath, image.ImageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        private static ProductAdminResponse MapToAdminResponse(Product p, string? categoryName) =>
            new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price ?? 0,
                Description = p.Description,
                CategoryName = categoryName ?? "N/A",
                Status = p.Status,
                CreatedAt = p.CreatedAt ?? DateTime.UtcNow,
                ImageUrl = p.ProductImages
                    .Where(img => img.IsMain == true)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault()
                    ?? p.ProductImages.Select(img => img.ImageUrl).FirstOrDefault()
            };
    }
}