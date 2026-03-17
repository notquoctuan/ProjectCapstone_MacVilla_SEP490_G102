using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers.Public;

/// <summary>
/// API dành cho khách hàng đã đăng nhập để xem và phản hồi báo giá.
/// </summary>
[ApiController]
[Route("api/customer/quotation")]
[Authorize(Roles = "Customer,Admin")]
public class CustomerQuotationController : ControllerBase
{
    private readonly IQuotationService _quotationService;

    public CustomerQuotationController(IQuotationService quotationService)
        => _quotationService = quotationService;

    /// <summary>Danh sách báo giá đã nhận (chỉ thấy trạng thái SentToCustomer, Approved, Rejected, Expired)</summary>
    /// <remarks>GET: /api/customer/quotation?status=SentToCustomer&amp;pageNumber=1&amp;pageSize=10</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<QuotationListResponse>>> GetMyQuotations(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _quotationService.GetMyQuotationsAsync(userId.Value, status, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>Xem chi tiết báo giá (kèm sản phẩm và tổng tiền)</summary>
    /// <remarks>GET: /api/customer/quotation/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<QuotationDetailResponse>> GetDetail([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });

        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _quotationService.GetMyQuotationDetailAsync(id, userId.Value);
        return result == null
            ? NotFound(new { message = $"Không tìm thấy báo giá #{id} của bạn." })
            : Ok(result);
    }

    /// <summary>Chấp nhận báo giá (SentToCustomer → Approved)</summary>
    /// <remarks>POST: /api/customer/quotation/{id}/approve</remarks>
    [HttpPost("{id:long}/approve")]
    public async Task<IActionResult> Approve([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });

        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        try
        {
            var ok = await _quotationService.ApproveQuotationAsync(id, userId.Value);
            return ok
                ? Ok(new { message = "Bạn đã chấp nhận báo giá. Nhân viên chúng tôi sẽ liên hệ sớm để tiến hành đơn hàng." })
                : NotFound(new { message = $"Không tìm thấy báo giá #{id} của bạn." });
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Từ chối báo giá (SentToCustomer → Rejected) — có thể kèm lý do</summary>
    /// <remarks>POST: /api/customer/quotation/{id}/reject</remarks>
    [HttpPost("{id:long}/reject")]
    public async Task<IActionResult> Reject(
        [FromRoute] long id,
        [FromBody] RejectQuotationRequest? request = null)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });

        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        try
        {
            var ok = await _quotationService.RejectQuotationAsync(id, userId.Value, request ?? new());
            return ok
                ? Ok(new { message = "Bạn đã từ chối báo giá." })
                : NotFound(new { message = $"Không tìm thấy báo giá #{id} của bạn." });
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
