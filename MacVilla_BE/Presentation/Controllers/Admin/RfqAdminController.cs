using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers.Admin;

/// <summary>
/// Admin/Sale quản lý các yêu cầu báo giá (RFQ) từ khách hàng.
/// </summary>
[ApiController]
[Route("api/admin/rfq")]
[Authorize(Roles = "Admin,Sale")]
public class RfqAdminController : ControllerBase
{
    private readonly IRfqService _rfqService;
    private readonly IQuotationService _quotationService;

    public RfqAdminController(IRfqService rfqService, IQuotationService quotationService)
    {
        _rfqService = rfqService;
        _quotationService = quotationService;
    }

    /// <summary>Dashboard Thống kê (Số lượng RFQ, Quotations, Doanh thu)</summary>
    /// <remarks>GET: /api/admin/rfq/dashboard</remarks>
    [HttpGet("dashboard")]
    public async Task<ActionResult<RfqDashboardStatsResponse>> GetDashboardStats()
    {
        var result = await _quotationService.GetDashboardStatsAsync();
        return Ok(result);
    }

    /// <summary>Danh sách RFQ — có filter (status, keyword, sale, ngày) và phân trang</summary>
    /// <remarks>GET: /api/admin/rfq?status=Pending&amp;keyword=Minh+Anh&amp;pageNumber=1&amp;pageSize=20</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<RfqListResponse>>> GetAll([FromQuery] RfqFilterRequest request)
    {
        var result = await _rfqService.GetAllRfqsAsync(request);
        return Ok(result);
    }

    /// <summary>Xem chi tiết 1 RFQ (kèm danh sách sản phẩm và báo giá đã tạo)</summary>
    /// <remarks>GET: /api/admin/rfq/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<RfqDetailResponse>> GetDetail([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "RFQ ID không hợp lệ." });

        var result = await _rfqService.GetRfqDetailAsync(id);
        return result == null
            ? NotFound(new { message = $"Không tìm thấy RFQ #{id}." })
            : Ok(result);
    }

    /// <summary>Cập nhật trạng thái RFQ (Pending → Processing → Quoted / Closed / Cancelled)</summary>
    /// <remarks>PATCH: /api/admin/rfq/{id}/status</remarks>
    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] long id,
        [FromBody] UpdateRfqStatusRequest request)
    {
        if (id <= 0) return BadRequest(new { message = "RFQ ID không hợp lệ." });

        try
        {
            var ok = await _rfqService.UpdateRfqStatusAsync(id, request);
            return ok
                ? Ok(new { message = "Cập nhật trạng thái thành công." })
                : NotFound(new { message = $"Không tìm thấy RFQ #{id}." });
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Gán nhân viên Sale phụ trách RFQ</summary>
    /// <remarks>PATCH: /api/admin/rfq/{id}/assign</remarks>
    [HttpPatch("{id:long}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignSale(
        [FromRoute] long id,
        [FromBody] AssignSaleRequest request)
    {
        if (id <= 0) return BadRequest(new { message = "RFQ ID không hợp lệ." });

        var ok = await _rfqService.AssignSaleAsync(id, request);
        return ok
            ? Ok(new { message = "Gán Sale thành công." })
            : NotFound(new { message = $"Không tìm thấy RFQ #{id}." });
    }

    /// <summary>Cập nhật ghi chú nội bộ của Sale/Admin</summary>
    /// <remarks>PATCH: /api/admin/rfq/{id}/note</remarks>
    [HttpPatch("{id:long}/note")]
    public async Task<IActionResult> UpdateNote(
        [FromRoute] long id,
        [FromBody] UpdateRfqInternalNoteRequest request)
    {
        if (id <= 0) return BadRequest(new { message = "RFQ ID không hợp lệ." });

        var ok = await _rfqService.UpdateInternalNoteAsync(id, request);
        return ok
            ? Ok(new { message = "Cập nhật ghi chú thành công." })
            : NotFound(new { message = $"Không tìm thấy RFQ #{id}." });
    }
}
