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
        public async Task<IActionResult> GetProductList([FromQuery] ProductSearchRequest request)
        {
            try
            {
                var result = await _productService.GetPagedProductsForAdminAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                string webRootPath = _webHostEnvironment.WebRootPath;

                var result = await _productService.CreateProductWithFilesAsync(request, webRootPath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo sản phẩm: " + ex.Message });
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                var result = await _productService.UpdateProductAsync(id, request, webRootPath);

                if (!result.Success)
                {
                    return result.Message.Contains("không tìm thấy")
                        ? NotFound(new { message = result.Message })
                        : BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
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