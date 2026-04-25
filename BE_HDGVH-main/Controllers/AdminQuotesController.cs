using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BE_API.Authorization;
using BE_API.Domain;
using BE_API.Dto.Admin;
using BE_API.Dto.Common;
using BE_API.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_API.Controllers;

[ApiController]
[Route("api/admin/quotes")]
[Authorize(Policy = Policies.StaffAuthenticated)]
public class AdminQuotesController(IAdminQuoteService quoteService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Danh sách báo giá (filter: status, customerId, salesId, fromDate, toDate, search)")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int? salesId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var data = await quoteService.GetPagedAsync(
            page, pageSize, status, customerId, salesId,
            fromDate, toDate, search, cancellationToken);

        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy danh sách báo giá thành công"
        });
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Chi tiết báo giá theo ID")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var data = await quoteService.GetByIdAsync(id, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy chi tiết báo giá thành công"
        });
    }

    [HttpGet("by-code/{quoteCode}")]
    [SwaggerOperation(Summary = "Chi tiết báo giá theo mã")]
    public async Task<IActionResult> GetByCode(string quoteCode, CancellationToken cancellationToken = default)
    {
        var data = await quoteService.GetByCodeAsync(quoteCode, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy chi tiết báo giá thành công"
        });
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo báo giá mới (Sales tạo cho khách B2B)")]
    public async Task<IActionResult> Create(
        [FromBody] AdminQuoteCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var salesId = GetCurrentUserId();
        if (!salesId.HasValue)
            return Unauthorized(new ResponseDto
            {
                Success = false,
                Message = "Không xác định được người dùng"
            });

        var data = await quoteService.CreateAsync(dto, salesId.Value, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Tạo báo giá thành công"
        });
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Cập nhật báo giá (chỉ khi Draft)")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] AdminQuoteUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var data = await quoteService.UpdateAsync(id, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Cập nhật báo giá thành công"
        });
    }

    [HttpPut("{id:int}/assign")]
    [SwaggerOperation(
        Summary = "Tiếp nhận / gán Sales: Requested hoặc CounterOffer → Draft",
        Description = "Body tùy chọn `{ \"salesId\": <int?> }`. Không gửi hoặc null → gán cho user hiện tại (Sales tiếp nhận). Có salesId khác user hiện tại → chỉ Manager hoặc admin.")]
    public async Task<IActionResult> Assign(
        int id,
        [FromBody] AdminQuoteAssignDto? dto,
        CancellationToken cancellationToken = default)
    {
        var callerId = GetCurrentUserId();
        if (!callerId.HasValue)
            return Unauthorized(new ResponseDto
            {
                Success = false,
                Message = "Không xác định được người dùng"
            });

        var targetSalesId = ResolveAssignSalesId(callerId.Value, dto?.SalesId);

        var data = await quoteService.AssignToSalesAsync(id, targetSalesId, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Tiếp nhận báo giá thành công"
        });
    }

    [HttpPut("{id:int}/return-to-draft")]
    [SwaggerOperation(Summary = "Đưa báo giá về nháp khi được phép (vd. CounterOffer, Rejected, PendingApproval → Draft). Giữ SalesId nếu đã có.")]
    public async Task<IActionResult> ReturnToDraft(int id, CancellationToken cancellationToken = default)
    {
        var staffId = GetCurrentUserId();
        if (!staffId.HasValue)
            return Unauthorized(new ResponseDto
            {
                Success = false,
                Message = "Không xác định được người dùng"
            });

        var data = await quoteService.ReturnToDraftAsync(id, staffId.Value, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Đã đưa báo giá về nháp"
        });
    }

    [HttpPut("{id:int}/submit")]
    [SwaggerOperation(Summary = "Gửi duyệt báo giá (Draft → PendingApproval)")]
    public async Task<IActionResult> Submit(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await quoteService.SubmitAsync(id, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Gửi duyệt báo giá thành công"
        });
    }

    [HttpPut("{id:int}/approve")]
    [Authorize(Policy = Policies.ManagerOrAdmin)]
    [SwaggerOperation(Summary = "Duyệt báo giá (Manager - PendingApproval → Approved)")]
    public async Task<IActionResult> Approve(
        int id,
        CancellationToken cancellationToken = default)
    {
        var managerId = GetCurrentUserId();
        if (!managerId.HasValue)
            return Unauthorized(new ResponseDto
            {
                Success = false,
                Message = "Không xác định được người dùng"
            });

        var data = await quoteService.ApproveAsync(id, managerId.Value, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Duyệt báo giá thành công"
        });
    }

    [HttpPut("{id:int}/reject")]
    [Authorize(Policy = Policies.ManagerOrAdmin)]
    [SwaggerOperation(Summary = "Từ chối báo giá (Manager - PendingApproval → Rejected)")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] AdminQuoteRejectDto dto,
        CancellationToken cancellationToken = default)
    {
        var managerId = GetCurrentUserId();
        if (!managerId.HasValue)
            return Unauthorized(new ResponseDto
            {
                Success = false,
                Message = "Không xác định được người dùng"
            });

        var data = await quoteService.RejectAsync(id, managerId.Value, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Từ chối báo giá thành công"
        });
    }

    [HttpPost("{id:int}/convert-to-order")]
    [SwaggerOperation(Summary = "Chuyển báo giá thành đơn (CustomerAccepted → Converted); tuỳ chọn ContractId đã Confirmed/Active")]
    public async Task<IActionResult> ConvertToOrder(
        int id,
        [FromBody] AdminQuoteConvertToOrderDto dto,
        CancellationToken cancellationToken = default)
    {
        var salesId = GetCurrentUserId();
        if (!salesId.HasValue)
            return Unauthorized(new ResponseDto
            {
                Success = false,
                Message = "Không xác định được người dùng"
            });

        var data = await quoteService.ConvertToOrderAsync(id, salesId.Value, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Chuyển báo giá thành đơn hàng thành công"
        });
    }

    [HttpPost("{id:int}/reserve-inventory")]
    [SwaggerOperation(Summary = "Giữ tồn theo báo giá (CustomerAccepted); mỗi dòng một giao dịch RESERVE")]
    public async Task<IActionResult> ReserveInventory(int id, CancellationToken cancellationToken = default)
    {
        var staffId = GetCurrentUserId();
        if (!staffId.HasValue)
            return Unauthorized(new ResponseDto { Success = false, Message = "Không xác định được người dùng" });

        var data = await quoteService.ReserveInventoryAsync(id, staffId.Value, cancellationToken);
        return Ok(new ResponseDto { Success = true, Data = data, Message = "Đã giữ tồn cho báo giá" });
    }

    [HttpPost("{id:int}/release-inventory-reservation")]
    [SwaggerOperation(Summary = "Trả giữ tồn đã gắn với báo giá (RELEASE)")]
    public async Task<IActionResult> ReleaseInventoryReservation(int id, CancellationToken cancellationToken = default)
    {
        var staffId = GetCurrentUserId();
        if (!staffId.HasValue)
            return Unauthorized(new ResponseDto { Success = false, Message = "Không xác định được người dùng" });

        var data = await quoteService.ReleaseInventoryReservationAsync(id, staffId.Value, cancellationToken);
        return Ok(new ResponseDto { Success = true, Data = data, Message = "Đã trả giữ tồn" });
    }

    [HttpGet("statuses")]
    [SwaggerOperation(Summary = "Danh sách các trạng thái báo giá")]
    public IActionResult GetQuoteStatuses()
    {
        return Ok(new ResponseDto
        {
            Success = true,
            Data = new
            {
                Statuses = QuoteStatuses.All
            },
            Message = "OK"
        });
    }

    private int? GetCurrentUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(sub, out var id) ? id : null;
    }

    /// <summary>
    /// Sales (và role khác): chỉ được gán cho chính mình. Manager / admin: có thể gán <paramref name="requestedSalesId"/> cho Sales khác.
    /// </summary>
    private int ResolveAssignSalesId(int callerId, int? requestedSalesId)
    {
        if (!requestedSalesId.HasValue || requestedSalesId.Value == callerId)
            return requestedSalesId ?? callerId;

        if (!IsManagerOrAdmin())
            throw new UnauthorizedAccessException("Chỉ Manager hoặc admin được gán báo giá cho nhân viên khác.");

        return requestedSalesId.Value;
    }

    private bool IsManagerOrAdmin()
    {
        var role = User.FindFirst(JwtClaimTypes.Role)?.Value ?? string.Empty;
        return string.Equals(role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase)
               || string.Equals(role, AppRoles.Manager, StringComparison.OrdinalIgnoreCase);
    }
}
