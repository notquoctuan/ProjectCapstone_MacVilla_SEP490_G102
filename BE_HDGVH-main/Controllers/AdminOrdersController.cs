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
[Route("api/admin/orders")]
[Authorize(Policy = Policies.StaffAuthenticated)]
public class AdminOrdersController(IAdminOrderService orderService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Danh sách đơn hàng (filter: orderStatus, paymentStatus, customerId, salesId, fromDate, toDate, search)")]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? orderStatus = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int? salesId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var data = await orderService.GetPagedAsync(
            page, pageSize, orderStatus, paymentStatus,
            customerId, salesId, fromDate, toDate, search,
            cancellationToken);

        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy danh sách đơn hàng thành công"
        });
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Chi tiết đơn hàng theo ID")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var data = await orderService.GetByIdAsync(id, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy chi tiết đơn hàng thành công"
        });
    }

    [HttpGet("by-code/{orderCode}")]
    [SwaggerOperation(Summary = "Chi tiết đơn hàng theo mã đơn")]
    public async Task<IActionResult> GetByCode(string orderCode, CancellationToken cancellationToken = default)
    {
        var data = await orderService.GetByCodeAsync(orderCode, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Lấy chi tiết đơn hàng thành công"
        });
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo đơn hàng (Sales tạo hộ khách tại quầy)")]
    public async Task<IActionResult> Create(
        [FromBody] AdminOrderCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var salesId = GetCurrentUserId();
        var data = await orderService.CreateAsync(dto, salesId, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Tạo đơn hàng thành công"
        });
    }

    [HttpPut("{id:int}/status")]
    [SwaggerOperation(Summary = "Cập nhật trạng thái đơn hàng (New → Confirmed → Processing → ReadyToShip → Shipped → Delivered → Completed)")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] AdminOrderUpdateStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var data = await orderService.UpdateStatusAsync(id, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Cập nhật trạng thái đơn hàng thành công"
        });
    }

    [HttpPut("{id:int}/payment-status")]
    [SwaggerOperation(Summary = "Cập nhật trạng thái thanh toán (Unpaid → PartiallyPaid → Paid)")]
    public async Task<IActionResult> UpdatePaymentStatus(
        int id,
        [FromBody] AdminOrderUpdatePaymentStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var data = await orderService.UpdatePaymentStatusAsync(id, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Cập nhật trạng thái thanh toán thành công"
        });
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Policy = Policies.ManagerOrAdmin)]
    [SwaggerOperation(Summary = "Hủy đơn hàng (Manager/Admin; cho phép ở New, AwaitingPayment, Confirmed, Processing, ReadyToShip)")]
    public async Task<IActionResult> Cancel(
        int id,
        [FromBody] AdminOrderCancelDto dto,
        CancellationToken cancellationToken = default)
    {
        var data = await orderService.CancelAsync(id, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Hủy đơn hàng thành công"
        });
    }

    [HttpPut("{id:int}/assign-sales")]
    [Authorize(Policy = Policies.ManagerOrAdmin)]
    [SwaggerOperation(Summary = "Gán nhân viên bán hàng cho đơn (Manager/Admin)")]
    public async Task<IActionResult> AssignSales(
        int id,
        [FromBody] AdminOrderAssignSalesDto dto,
        CancellationToken cancellationToken = default)
    {
        var data = await orderService.AssignSalesAsync(id, dto, cancellationToken);
        return Ok(new ResponseDto
        {
            Success = true,
            Data = data,
            Message = "Gán nhân viên bán hàng thành công"
        });
    }

    [HttpGet("{id:int}/timeline")]
    [SwaggerOperation(Summary = "Timeline đơn hàng (admin) — sự kiện thực từ đơn, phiếu xuất, thanh toán, hóa đơn, CK, đổi trả")]
    public async Task<IActionResult> GetTimelineById(int id, CancellationToken cancellationToken = default)
    {
        var data = await orderService.GetTimelineByIdAsync(id, cancellationToken);
        return Ok(new ResponseDto { Success = true, Data = data, Message = "OK" });
    }

    [HttpGet("by-code/{orderCode}/timeline")]
    [SwaggerOperation(Summary = "Timeline đơn hàng theo mã (admin)")]
    public async Task<IActionResult> GetTimelineByCode(string orderCode, CancellationToken cancellationToken = default)
    {
        var data = await orderService.GetTimelineByCodeAsync(orderCode, cancellationToken);
        return Ok(new ResponseDto { Success = true, Data = data, Message = "OK" });
    }

    [HttpGet("statuses")]
    [SwaggerOperation(Summary = "Danh sách các trạng thái đơn hàng")]
    public IActionResult GetOrderStatuses()
    {
        return Ok(new ResponseDto
        {
            Success = true,
            Data = new
            {
                OrderStatuses = OrderStatuses.All,
                PaymentStatuses = PaymentStatuses.All
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
}
