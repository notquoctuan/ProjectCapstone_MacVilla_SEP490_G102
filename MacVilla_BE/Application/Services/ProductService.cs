using Domain.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public async Task<ProductAdminResponse> CreateProductWithFilesAsync(ProductCreateRequest request, string webRootPath)
        {
            // 1. Kiểm tra Category
            var categories = await _categoryRepository.GetAllAsync();
            if (!categories.Any(c => c.CategoryId == request.CategoryId))
                throw new Exception("Danh mục không tồn tại.");

            // 2. Tạo Entity Product
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                CategoryId = request.CategoryId,
                Description = request.Description,
                Status = request.Status,
                CreatedAt = DateTime.Now
            };

            // 3. Xử lý lưu File ảnh
            if (request.ImageFiles != null && request.ImageFiles.Count > 0)
            {
                var uploadPath = Path.Combine(webRootPath, "uploads", "products");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                for (int i = 0; i < request.ImageFiles.Count; i++)
                {
                    var file = request.ImageFiles[i];
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = $"/uploads/products/{fileName}",
                        IsMain = (i == 0) // Ảnh đầu tiên là ảnh chính
                    });
                }
            }

            var savedProduct = await _productRepository.AddAsync(product);

            return new ProductAdminResponse
            {
                ProductId = savedProduct.ProductId,
                Name = savedProduct.Name,
                Price = savedProduct.Price ?? 0,
                Status = savedProduct.Status ?? "Pending",
                CategoryName = categories.FirstOrDefault(c => c.CategoryId == savedProduct.CategoryId)?.CategoryName
            };
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

        public async Task<PagedResponse<ProductAdminResponse>> GetPagedProductsForAdminAsync(ProductSearchRequest request)
        {
            // 1. Lấy Queryable gốc
            var query = _productRepository.GetQueryable();

            // 2. Lọc theo tên sản phẩm (nếu có)
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(p => p.Name.Contains(request.Name));
            }

            // 3. Lọc theo Tên danh mục (nếu có)
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                query = query.Where(p => p.Category != null && p.Category.CategoryName.Contains(request.CategoryName));
            }

            // 4. Lọc theo khoảng giá
            if (request.MinPrice.HasValue) query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue) query = query.Where(p => p.Price <= request.MaxPrice.Value);

            // 5. Xử lý Sắp xếp theo giá
            query = request.SortOrder switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderByDescending(p => p.CreatedAt) // Mặc định sắp xếp theo ngày tạo mới nhất
            };

            // 6. Tính tổng số bản ghi sau khi đã lọc (để phân trang ở FE)
            int totalCount = await query.CountAsync();

            // 7. Thực hiện phân trang và Map dữ liệu sang Response DTO
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductAdminResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price ?? 0,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                    Status = p.Status,
                    CreatedAt = p.CreatedAt ?? DateTime.Now,
                    ImageUrl = p.ProductImages
                        .OrderByDescending(img => img.IsMain)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

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
            // 1. Tìm sản phẩm cũ kèm danh sách ảnh (phải Include ProductImages)
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return new UpdateResult { Success = false, Message = "Không tìm thấy sản phẩm." };

            // 2. Kiểm tra danh mục hợp lệ (Bảo mật tầng BE)
            var categoryExists = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (categoryExists == null)
                return new UpdateResult { Success = false, Message = "Danh mục không hợp lệ." };

            // 3. Tiến hành cập nhật và kiểm tra thay đổi
            existingProduct.Name = request.Name;
            existingProduct.Price = request.Price;
            existingProduct.Description = request.Description;
            existingProduct.CategoryId = request.CategoryId;
            existingProduct.Status = request.Status;

            // 4. Xử lý ảnh mới (Chỉ thực hiện nếu có file upload)
            if (request.NewImageFiles != null && request.NewImageFiles.Any())
            {
                // A. XÓA FILE ẢNH VẬT LÝ TRÊN SERVER (Tránh rác bộ nhớ)
                foreach (var oldImg in existingProduct.ProductImages)
                {
                    var oldPath = Path.Combine(webRootPath, oldImg.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }

                // B. Xóa dữ liệu ảnh cũ trong DB
                existingProduct.ProductImages.Clear();

                // C. Lưu file mới vào thư mục
                string uploadsFolder = Path.Combine(webRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                bool isFirst = true;
                foreach (var file in request.NewImageFiles)
                {
                    string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // D. Thêm vào danh sách ảnh mới
                    existingProduct.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = "/uploads/products/" + fileName,
                        IsMain = isFirst // Tự động gán cái đầu tiên làm ảnh chính
                    });
                    isFirst = false;
                }
            }

            // 5. Lưu vào Database thông qua Repository
            try
            {
                await _productRepository.UpdateAsync(existingProduct);
                return new UpdateResult { Success = true, Message = "Cập nhật sản phẩm thành công." };
            }
            catch (Exception ex)
            {
                return new UpdateResult { Success = false, Message = "Lỗi hệ thống: " + ex.Message };
            }
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