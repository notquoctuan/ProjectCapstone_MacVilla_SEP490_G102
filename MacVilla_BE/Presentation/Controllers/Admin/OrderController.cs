using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get all orders with search, filter, and pagination
    /// </summary>
    /// <remarks>GET: api/admin/order?status=Processing&amp;pageNumber=1&amp;pageSize=10</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderListResponse>>> GetAll([FromQuery] OrderSearchRequest request)
    {
        try
        {
            var result = await _orderService.SearchOrdersAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get order detail by ID
    /// </summary>
    /// <remarks>GET: api/admin/order/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrderDetail([FromRoute] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Order ID must be greater than 0." });
        }

        var order = await _orderService.GetOrderDetailAsync(id);
        if (order == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        return Ok(order);
    }

    /// <summary>
    /// Get order tracking information
    /// </summary>
    /// <remarks>GET: api/admin/order/{id}/tracking</remarks>
    [HttpGet("{id:long}/tracking")]
    public async Task<ActionResult<OrderTrackingResponse>> GetOrderTracking([FromRoute] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Order ID must be greater than 0." });
        }

        var tracking = await _orderService.GetOrderTrackingAsync(id);
        if (tracking == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        return Ok(tracking);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    /// <remarks>PUT: api/admin/order/{id}/status</remarks>
    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] long id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Order ID must be greater than 0." });
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Status is required." });
        }

        try
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, request);
            if (!success)
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(new { message = "Order status updated successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating order status.", error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    /// <remarks>POST: api/admin/order/{id}/cancel</remarks>
    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> CancelOrder(
        [FromRoute] long id,
        [FromBody] CancelOrderRequest? request = null)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Order ID must be greater than 0." });
        }

        try
        {
            var reason = request?.Reason;
            var success = await _orderService.CancelOrderAsync(id, reason);
            if (!success)
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(new { message = "Order cancelled successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while cancelling order.", error = ex.Message });
        }
    }
}

public class CancelOrderRequest
{
    public string? Reason { get; set; }
}