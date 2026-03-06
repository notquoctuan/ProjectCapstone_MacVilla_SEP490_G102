using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Public;

[ApiController]
[Route("api/home")]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet("banners")]
    public async Task<IActionResult> GetBanners()
    {
        try
        {
            var banners = await _homeService.GetActiveBannersAsync();
            return Ok(banners);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi hiển thị banner", detail = ex.Message });
        }
    }

    [HttpGet("featured-products")]
    public async Task<IActionResult> GetFeaturedProducts()
    {
        try
        {
            var products = await _homeService.GetFeaturedProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi hiển thị sản phẩm", detail = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchPublicRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Keyword))
                return BadRequest(new { message = "Keyword cannot be empty." });

            var result = await _homeService.SearchProductsAsync(request);
            return Ok(result);
        }
        catch (ArgumentException aex)
        {
            return BadRequest(new { message = aex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi tìm kiếm", detail = ex.Message });
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var tree = await _homeService.GetCategoryTreeAsync();
            return Ok(tree);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi hiển thị danh mục", detail = ex.Message });
        }
    }
}
