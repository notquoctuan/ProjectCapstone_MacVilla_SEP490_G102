using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Public;

/// <summary>
/// API công khai — không cần đăng nhập.
/// Dùng cho trang chủ, trang sản phẩm, trang danh mục.
/// </summary>
[ApiController]
[Route("api/products")]
public class ProductPublicController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductPublicController(IProductService productService)
        => _productService = productService;

    /// <summary>Lấy danh sách sản phẩm (lọc, phân trang) — chỉ trả sản phẩm Enable</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductSearchRequest request)
    {
        // Khách hàng chỉ thấy sản phẩm đang bán
        request.Status = "Enable";

        try
        {
            var result = await _productService.GetPagedProductsAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Xem chi tiết 1 sản phẩm</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID sản phẩm không hợp lệ." });

        var product = await _productService.GetProductDetailAsync(id);
        if (product == null)
            return NotFound(new { message = "Không tìm thấy sản phẩm." });

        // Không trả về sản phẩm đã bị ẩn hoặc pending
        if (product.Status != "Enable")
            return NotFound(new { message = "Không tìm thấy sản phẩm." });

        return Ok(product);
    }
}