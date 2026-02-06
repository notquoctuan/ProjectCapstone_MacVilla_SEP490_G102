using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; 

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/products")]
    //[Authorize(Roles = "Admin")]
    public class ProductAdminController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductAdminController(ProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
        }
       
        /// <summary>
        /// Lấy danh sách sản phẩm hiển thị cho giao diện Admin
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            try
            {
                var products = await _productService.GetAllProductsForAdmin();

                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm nào." });
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống", detail = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(long id)
        {
            var product = await _productService.GetProductDetailAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm yêu cầu." });
            }
            return Ok(product);
        }
        /// <summary>
        /// Tìm kiếm, lọc và phân trang sản phẩm cho Admin
        /// </summary>
        [HttpGet("filter")]
        public async Task<ActionResult<PagedResponse<ProductAdminResponse>>> FilterProducts([FromQuery] ProductSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
            {
                return BadRequest(new { message = "Giá tối thiểu không được lớn hơn giá tối đa." });
            }

            try
            {
                var result = await _productService.GetPagedProductsForAdminAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý dữ liệu.", detail = ex.Message });
            }
        }
        [HttpPost("create-with-image")]
        [Consumes("multipart/form-data")] // Chỉ định nhận dữ liệu form-data
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Lấy đường dẫn wwwroot để lưu file
                string webRootPath = _webHostEnvironment.WebRootPath;
                var result = await _productService.CreateProductWithFilesAsync(request, webRootPath);

                return Ok(new { message = "Tạo sản phẩm và tải ảnh thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] ProductUpdateRequest request)
        {
            try
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                var result = await _productService.UpdateProductAsync(id, request, webRootPath);

                if (!result.Success) return NotFound(new { message = result.Message });

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(long id, [FromQuery] string status)
        {
            try
            {
                var result = await _productService.ChangeProductStatusAsync(id, status);
                if (!result) return NotFound(new { message = "Không tìm thấy sản phẩm." });

                return Ok(new { message = $"Đã chuyển trạng thái sản phẩm sang {status}." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}