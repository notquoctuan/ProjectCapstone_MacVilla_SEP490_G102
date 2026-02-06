using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin")]
    public class ProductAdminController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductAdminController(ProductService productService)
        {
            _productService = productService;
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
    }
}