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

        
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? categoryId)
        {
            var result = await _productService.SearchProductsForAdmin(name, minPrice, maxPrice, categoryId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _productService.GetProductDetailsAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có ID: {id}" });
            }

            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductRequest request)
        {
            var success = await _productService.UpdateProductAsync(id, request);

            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm ID {id} để cập nhật." });
            }

            return Ok(new { message = "Cập nhật sản phẩm thành công!" });
        }
        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> DisableProduct(long id)
        {
            var result = await _productService.DisableProductAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có ID: {id}" });
            }

            return Ok(new { message = "Đã vô hiệu hóa sản phẩm thành công." });
        }
    }
}