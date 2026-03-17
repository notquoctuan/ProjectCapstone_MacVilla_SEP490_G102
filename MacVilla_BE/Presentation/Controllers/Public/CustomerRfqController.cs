using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers.Public;

/// <summary>
/// API dành cho khách hàng đã đăng nhập để gửi và quản lý yêu cầu báo giá.
/// </summary>
[ApiController]
[Route("api/customer/rfq")]
[Authorize(Roles = "Customer,Admin")]
public class CustomerRfqController : ControllerBase
{
    private readonly IRfqService _rfqService;

    public CustomerRfqController(IRfqService rfqService)
        => _rfqService = rfqService;

    /// <summary>Gửi yêu cầu báo giá mới</summary>
    /// <remarks>POST: /api/customer/rfq</remarks>
    [HttpPost]
    public async Task<ActionResult<RfqDetailResponse>> Create([FromBody] CreateRfqRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        try
        {
            var result = await _rfqService.CreateRfqAsync(request, userId);
            return CreatedAtAction(nameof(GetDetail), new { id = result.RfqId }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi tạo yêu cầu báo giá.", error = ex.Message });
        }
    }

    /// <summary>Danh sách yêu cầu báo giá của tôi</summary>
    /// <remarks>GET: /api/customer/rfq?status=Pending&amp;pageNumber=1&amp;pageSize=10</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<RfqListResponse>>> GetMyRfqs(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _rfqService.GetMyRfqsAsync(userId.Value, status, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>Xem chi tiết yêu cầu báo giá của tôi</summary>
    /// <remarks>GET: /api/customer/rfq/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<RfqDetailResponse>> GetDetail([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "RFQ ID không hợp lệ." });

        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _rfqService.GetMyRfqDetailAsync(id, userId.Value);
        return result == null
            ? NotFound(new { message = $"Không tìm thấy yêu cầu báo giá #{id} của bạn." })
            : Ok(result);
    }

    /// <summary>Hủy yêu cầu báo giá (chỉ khi đang Pending)</summary>
    /// <remarks>POST: /api/customer/rfq/{id}/cancel</remarks>
    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "RFQ ID không hợp lệ." });

        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        try
        {
            var ok = await _rfqService.CancelRfqAsync(id, userId.Value);
            return ok
                ? Ok(new { message = "Đã hủy yêu cầu báo giá thành công." })
                : NotFound(new { message = $"Không tìm thấy yêu cầu báo giá #{id} của bạn." });
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private long? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
        return long.TryParse(claim, out var id) ? id : null;
    }
}
