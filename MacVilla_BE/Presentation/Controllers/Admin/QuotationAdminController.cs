using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers.Admin;

/// <summary>
/// Admin/Sale tạo và quản lý báo giá (Quotation) gửi cho khách hàng.
/// </summary>
[ApiController]
[Route("api/admin/quotation")]
[Authorize(Roles = "Admin,Sale")]
public class QuotationAdminController : ControllerBase
{
    private readonly IQuotationService _quotationService;

    public QuotationAdminController(IQuotationService quotationService)
        => _quotationService = quotationService;

    /// <summary>Danh sách báo giá — có filter và phân trang</summary>
    /// <remarks>GET: /api/admin/quotation?status=Draft&amp;keyword=QT-2024&amp;pageNumber=1&amp;pageSize=20</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<QuotationListResponse>>> GetAll([FromQuery] QuotationFilterRequest request)
    {
        var result = await _quotationService.GetAllQuotationsAsync(request);
        return Ok(result);
    }

    /// <summary>Chi tiết báo giá (kèm danh sách sản phẩm + tài chính)</summary>
    /// <remarks>GET: /api/admin/quotation/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<QuotationDetailResponse>> GetDetail([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });

        var result = await _quotationService.GetQuotationDetailAsync(id);
        return result == null
            ? NotFound(new { message = $"Không tìm thấy báo giá #{id}." })
            : Ok(result);
    }

    /// <summary>Tạo báo giá mới từ một RFQ</summary>
    /// <remarks>POST: /api/admin/quotation</remarks>
    [HttpPost]
    public async Task<ActionResult<QuotationDetailResponse>> Create([FromBody] CreateQuotationRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdBy = GetCurrentUserId();
        if (createdBy == null)
            return Unauthorized(new { message = "Không xác định được người dùng." });

        try
        {
            var result = await _quotationService.CreateQuotationAsync(request, createdBy.Value);
            return CreatedAtAction(nameof(GetDetail), new { id = result.QuotationId }, result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Cập nhật nội dung báo giá (chỉ khi đang ở trạng thái Draft)</summary>
    /// <remarks>PUT: /api/admin/quotation/{id}</remarks>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<QuotationDetailResponse>> Update(
        [FromRoute] long id,
        [FromBody] UpdateQuotationRequest request)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var result = await _quotationService.UpdateQuotationAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Gửi báo giá cho khách hàng (Draft → SentToCustomer)</summary>
    /// <remarks>POST: /api/admin/quotation/{id}/send</remarks>
    [HttpPost("{id:long}/send")]
    public async Task<IActionResult> Send([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });

        try
        {
            var ok = await _quotationService.SendQuotationAsync(id);
            return ok
                ? Ok(new { message = "Đã gửi báo giá cho khách hàng thành công." })
                : NotFound(new { message = $"Không tìm thấy báo giá #{id}." });
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Tải xuống file PDF của báo giá</summary>
    /// <remarks>GET: /api/admin/quotation/{id}/pdf</remarks>
    [HttpGet("{id:long}/pdf")]
    public async Task<IActionResult> ExportPdf([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });

        try
        {
            var pdfBytes = await _quotationService.ExportPdfAsync(id);

            // Trả về file PDF để tải về
            return File(pdfBytes, "application/pdf", $"BaoGia-{id}.pdf");
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = "Lỗi khi tạo PDF.", error = ex.Message }); }
    }

    // ── Nâng cao: Hủy, Điều chỉnh (Clone), Tạo đơn hàng ────────────────────

    /// <summary>Hủy báo giá</summary>
    /// <remarks>POST: /api/admin/quotation/{id}/cancel</remarks>
    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });
        try
        {
            var ok = await _quotationService.CancelQuotationAsync(id);
            return ok 
                ? Ok(new { message = "Hủy báo giá thành công." }) 
                : NotFound(new { message = $"Không tìm thấy báo giá #{id}." });
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Tạo bản sao điều chỉnh (Clone) từ báo giá hiện có</summary>
    /// <remarks>POST: /api/admin/quotation/{id}/revise</remarks>
    [HttpPost("{id:long}/revise")]
    public async Task<ActionResult<QuotationDetailResponse>> Revise([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });
        var adminId = GetCurrentUserId();
        if (adminId == null) return Unauthorized();

        try
        {
            var result = await _quotationService.ReviseQuotationAsync(id, adminId.Value);
            return Ok(new { message = "Tạo báo giá điều chỉnh thành công.", data = result });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = "Lỗi khi tạo bản điều chỉnh.", error = ex.Message }); }
    }

    /// <summary>Tạo đơn hàng từ báo giá đã chốt (Approved)</summary>
    /// <remarks>POST: /api/admin/quotation/{id}/create-order</remarks>
    [HttpPost("{id:long}/create-order")]
    public async Task<IActionResult> CreateOrder([FromRoute] long id)
    {
        if (id <= 0) return BadRequest(new { message = "Quotation ID không hợp lệ." });
        var adminId = GetCurrentUserId();
        if (adminId == null) return Unauthorized();

        try
        {
            var orderId = await _quotationService.CreateOrderFromQuotationAsync(id, adminId.Value);
            return Ok(new { message = "Tạo đơn hàng thành công.", orderId = orderId });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = "Lỗi hệ thống khi tạo đơn.", error = ex.Message }); }
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private long? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
        return long.TryParse(claim, out var id) ? id : null;
    }
}
