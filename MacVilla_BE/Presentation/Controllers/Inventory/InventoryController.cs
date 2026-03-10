using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Inventory;

[ApiController]
[Route("api/admin/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Danh sách tồn kho (tìm kiếm, phân trang).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<InventorySummaryResponse>>> GetAll(
        [FromQuery] InventorySearchRequest request)
    {
        var result = await _inventoryService.SearchInventoriesAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Lấy tồn kho theo ID tồn kho.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<InventoryDetailResponse>> GetById([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "ID tồn kho phải lớn hơn 0." });

        var inventory = await _inventoryService.GetByInventoryIdAsync(id);
        if (inventory == null) return NotFound();

        return Ok(inventory);
    }

    /// <summary>
    /// Lấy tồn kho theo sản phẩm.
    /// </summary>
    [HttpGet("product/{productId:long}")]
    public async Task<ActionResult<InventoryDetailResponse>> GetByProduct([FromRoute] long productId)
    {
        if (productId <= 0) return BadRequest(new { message = "ID sản phẩm phải lớn hơn 0." });

        var inventory = await _inventoryService.GetByProductIdAsync(productId);
        if (inventory == null) return NotFound();

        return Ok(inventory);
    }

    /// <summary>
    /// Lịch sử nhập/xuất tồn kho.
    /// </summary>
    [HttpGet("{id:long}/history")]
    public async Task<ActionResult<IEnumerable<InventoryHistoryDto>>> GetHistory([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "ID tồn kho phải lớn hơn 0." });

        var history = await _inventoryService.GetHistoryAsync(id);
        return Ok(history);
    }

    /// <summary>
    /// Thống kê tổng quan tồn kho.
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<InventoryStatisticsResponse>> GetStatistics()
    {
        var stats = await _inventoryService.GetStatisticsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Cập nhật trực tiếp số lượng tồn cho một sản phẩm.
    /// </summary>
    [HttpPut("product/{productId:long}")]
    public async Task<ActionResult<InventoryDetailResponse>> UpdateInventory(
        [FromRoute] long productId,
        [FromBody] UpdateInventoryRequest request)
    {
        if (productId <= 0) return BadRequest(new { message = "ID sản phẩm phải lớn hơn 0." });

        try
        {
            var result = await _inventoryService.UpdateInventoryAsync(productId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Điều chỉnh tồn kho (nhập/xuất kho).
    /// </summary>
    [HttpPost("product/{productId:long}/adjust")]
    public async Task<ActionResult<InventoryDetailResponse>> AdjustInventory(
        [FromRoute] long productId,
        [FromBody] AdjustInventoryRequest request)
    {
        if (productId <= 0) return BadRequest(new { message = "ID sản phẩm phải lớn hơn 0." });

        try
        {
            var result = await _inventoryService.AdjustInventoryAsync(productId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}