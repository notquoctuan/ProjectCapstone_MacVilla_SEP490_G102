using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers.Public;

/// <summary>
/// API đơn hàng cho customer — chỉ trả về đơn của chính user đó.
/// </summary>
[ApiController]
[Route("api/my/orders")]
[Authorize]
public class OrderCustomerController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderCustomerController(IOrderService orderService)
        => _orderService = orderService;

    private long CurrentUserId
    {
        get
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(claim) && long.TryParse(claim, out var id))
                return id;
            throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong token.");
        }
    }

    /// <summary>Lấy danh sách đơn hàng của tôi</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] OrderSearchRequest request)
    {
        try
        {
            // Bắt buộc lọc theo userId hiện tại — không cho phép xem đơn người khác
            request.UserId = CurrentUserId;
            var result = await _orderService.SearchOrdersAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Xem chi tiết 1 đơn hàng của tôi</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetOrderDetail(long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Order ID không hợp lệ." });

        try
        {
            var order = await _orderService.GetOrderDetailAsync(id);
            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            // Chỉ cho xem đơn của chính mình
            if (order.Customer?.UserId != CurrentUserId)
                return Forbid();

            return Ok(order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Theo dõi trạng thái đơn hàng của tôi</summary>
    [HttpGet("{id:long}/tracking")]
    public async Task<IActionResult> TrackOrder(long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Order ID không hợp lệ." });

        try
        {
            var detail = await _orderService.GetOrderDetailAsync(id);
            if (detail == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            if (detail.Customer?.UserId != CurrentUserId)
                return Forbid();

            var tracking = await _orderService.GetOrderTrackingAsync(id);
            return Ok(tracking);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Huỷ đơn hàng của tôi (chỉ khi còn Pending)</summary>
    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> CancelMyOrder(long id, [FromBody] CancelMyOrderRequest? request = null)
    {
        if (id <= 0)
            return BadRequest(new { message = "Order ID không hợp lệ." });

        try
        {
            // Kiểm tra đơn thuộc về user này
            var detail = await _orderService.GetOrderDetailAsync(id);
            if (detail == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            if (detail.Customer?.UserId != CurrentUserId)
                return Forbid();

            // Customer chỉ huỷ được khi đơn còn Pending
            if (detail.Status != "Pending")
                return BadRequest(new { message = "Chỉ có thể huỷ đơn hàng đang ở trạng thái Chờ xử lý (Pending)." });

            var success = await _orderService.CancelOrderAsync(id, request?.Reason);
            if (!success)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            return Ok(new { message = "Đã huỷ đơn hàng thành công." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CancelMyOrderRequest(string? Reason);