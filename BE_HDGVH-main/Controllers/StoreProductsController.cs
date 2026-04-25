using BE_API.Dto.Common;
using BE_API.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_API.Controllers;

[ApiController]
[Route("api/store/products")]
[AllowAnonymous]
public class StoreProductsController(IStoreCatalogService storeCatalog) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Danh sách sản phẩm Active; mỗi item có ImageUrl (SP hoặc fallback variant Id nhỏ nhất có ảnh)")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] int? categoryId = null,
        [FromQuery] bool includeSubcategories = true,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var data = await storeCatalog.GetProductsPagedAsync(
            page, pageSize, categoryId, includeSubcategories, search, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy danh sách sản phẩm thành công"
        });
    }

    [HttpGet("id/{id:int}")]
    [SwaggerOperation(Summary = "Chi tiết sản phẩm Active theo id; có ImageUrl đại diện + ảnh từng variant")]
    public async Task<IActionResult> GetDetailById(int id, CancellationToken cancellationToken = default)
    {
        var data = await storeCatalog.GetProductDetailByIdAsync(id, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy sản phẩm thành công"
        });
    }

    /// <summary>Ưu tiên theo id nếu <paramref name="slugOrId"/> là số nguyên và tồn tại SP Active; không thì theo slug.</summary>
    [HttpGet("{slugOrId}")]
    [SwaggerOperation(Summary = "Chi tiết sản phẩm Active theo slug hoặc id: ImageUrl đại diện, thuộc tính, biến thể (không giá vốn), tồn")]
    public async Task<IActionResult> GetDetail(string slugOrId, CancellationToken cancellationToken = default)
    {
        var data = await storeCatalog.GetProductBySlugOrIdAsync(slugOrId, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy sản phẩm thành công"
        });
    }
}
