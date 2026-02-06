using Domain.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Tạo sản phẩm mới kèm theo upload ảnh từ máy tính và gán danh mục
        /// </summary>
        public async Task<ProductAdminResponse> CreateProductWithFilesAsync(ProductCreateRequest request, string webRootPath)
        {
            // 1. Kiểm tra Category
            var categories = await _categoryRepository.GetAllAsync();
            if (!categories.Any(c => c.CategoryId == request.CategoryId))
                throw new Exception("Danh mục không tồn tại.");

            // 2. Khởi tạo Entity
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                CategoryId = request.CategoryId,
                Description = request.Description,
                Status = request.Status,
                CreatedAt = DateTime.Now,
                ProductImages = new List<ProductImage>()
            };

            // 3. Xử lý lưu ảnh
            if (request.ImageFiles != null && request.ImageFiles.Any())
            {
                // Đường dẫn vật lý để ghi file (Có wwwroot)
                string uploadsFolder = Path.Combine(webRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in request.ImageFiles)
                {
                    // Tạo tên file duy nhất để tránh trùng lặp
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // LƯU VÀO DB: Chỉ lưu đường dẫn tương đối (KHÔNG có wwwroot)
                    // Dấu '/' ở đầu rất quan trọng để FE hiểu đây là đường dẫn từ gốc domain
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = "/uploads/products/" + fileName,
                        IsMain = product.ProductImages.Count == 0
                    });
                }
            }

            await _productRepository.AddAsync(product);
            return new ProductAdminResponse { ProductId = product.ProductId, Name = product.Name };
        }

        /// <summary>
        /// Lấy toàn bộ danh sách sản phẩm cho Admin (không phân trang)
        /// </summary>
        public async Task<IEnumerable<ProductAdminResponse>> GetAllProductsForAdmin()
        {
            var products = await _productRepository.GetProductsForAdminAsync();

            return products.Select(p => new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                ImageUrl = p.ProductImages != null
                    ? p.ProductImages.OrderByDescending(img => img.IsMain)
                                     .Select(img => img.ImageUrl)
                                     .FirstOrDefault()
                    : null
            });
        }

        /// <summary>
        /// Tìm kiếm, lọc và phân trang danh sách sản phẩm cho Admin
        /// </summary>
        public async Task<PagedResponse<ProductAdminResponse>> GetPagedProductsForAdminAsync(ProductSearchRequest request)
        {
            // Lấy dữ liệu thô từ Repository (đã bao gồm Include Category và Images)
            var products = await _productRepository.GetProductsForAdminAsync();
            var query = products.AsQueryable();

            // 1. Lọc theo tên sản phẩm
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(p => p.Name != null && p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            }

            // 2. Lọc theo khoảng giá
            if (request.MinPrice.HasValue) query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue) query = query.Where(p => p.Price <= request.MaxPrice.Value);

            // 3. Mapping sang DTO (Đã sửa lỗi CS8072 - không dùng ?. trong Expression)
            var mappedData = query.Select(p => new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                ImageUrl = p.ProductImages != null
                    ? p.ProductImages.OrderByDescending(img => img.IsMain)
                                     .Select(img => img.ImageUrl)
                                     .FirstOrDefault()
                    : null
            });

            // 4. Lọc theo tên danh mục (thực hiện sau khi đã map hoặc truy cập qua navigation property)
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                mappedData = mappedData.Where(r => r.CategoryName != null &&
                    r.CategoryName.Contains(request.CategoryName, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Thực hiện phân trang và tính toán tổng số bản ghi
            int totalCount = mappedData.Count();
            var items = mappedData
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Trả về kết quả bọc trong PagedResponse
            return new PagedResponse<ProductAdminResponse>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        public async Task<UpdateResult> UpdateProductAsync(long id, ProductUpdateRequest request, string webRootPath)
        {
            // 1. Tìm sản phẩm cũ
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return new UpdateResult { Success = false, Message = "Không tìm thấy sản phẩm." };

            // 2. Kiểm tra xem có bất kỳ sự thay đổi nào không
            bool isChanged = false;

            if (existingProduct.Name != request.Name) isChanged = true;
            if (existingProduct.Price != request.Price) isChanged = true;
            if (existingProduct.Description != request.Description) isChanged = true;
            if (existingProduct.CategoryId != request.CategoryId) isChanged = true;
            if (existingProduct.Status != request.Status) isChanged = true;

            // Kiểm tra nếu có upload ảnh mới
            if (request.NewImageFiles != null && request.NewImageFiles.Any()) isChanged = true;

            // 3. Nếu không có gì thay đổi, trả về thông báo ngay
            if (!isChanged)
            {
                return new UpdateResult { Success = true, Message = "Dữ liệu giống hệt bản cũ, không có gì để cập nhật." };
            }

            // 4. Nếu có thay đổi, tiến hành gán giá trị mới
            existingProduct.Name = request.Name;
            existingProduct.Price = request.Price;
            existingProduct.Description = request.Description;
            existingProduct.CategoryId = request.CategoryId;
            existingProduct.Status = request.Status;

            // Xử lý ảnh mới (nếu có)
            if (request.NewImageFiles != null && request.NewImageFiles.Any())
            {
                existingProduct.ProductImages.Clear();
                string uploadsFolder = Path.Combine(webRootPath, "uploads", "products");
                foreach (var file in request.NewImageFiles)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    existingProduct.ProductImages.Add(new ProductImage { ImageUrl = "/uploads/products/" + fileName });
                }
            }

            await _productRepository.UpdateAsync(existingProduct);
            return new UpdateResult { Success = true, Message = "Cập nhật sản phẩm thành công." };
        }
        public async Task<bool> ChangeProductStatusAsync(long id, string newStatus)
        {
            // Kiểm tra giá trị status có nằm trong danh sách cho phép không
            var validStatuses = new[] { "Disable", "Enable", "Pending" };
            if (!validStatuses.Contains(newStatus))
            {
                throw new Exception("Trạng thái không hợp lệ. Chỉ chấp nhận: Disable, Enable, Pending.");
            }

            return await _productRepository.UpdateStatusAsync(id, newStatus);
        }
        // Tại ProductService.cs
        public async Task<ProductDetailResponse?> GetProductDetailAsync(long id)
        {
            var product = await _productRepository.GetByIdDetailAsync(id);
            if (product == null) return null;

            return new ProductDetailResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price ?? 0m,
                Description = product.Description,
                Status = product.Status ?? "Pending",
                CreatedAt = product.CreatedAt ?? DateTime.Now,
                CategoryId = product.CategoryId ?? 0,
                CategoryName = product.Category?.CategoryName,
                Images = product.ProductImages.Select(img => new ProductImageResponse
                {
                    // Kiểm tra tên thuộc tính ID của ProductImage
                    ImageId = img.ImageId,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain ?? false // Fix lỗi bool? sang bool
                }).ToList()
            };
        }
    }
}