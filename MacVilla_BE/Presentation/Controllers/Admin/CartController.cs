using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
        => _cartService = cartService;

    private long CurrentUserId
    {
        get
        {
            // Thử lấy từ NameIdentifier trước
            var nameId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(nameId) && long.TryParse(nameId, out var userId))
                return userId;

            // Fallback: thử lấy từ "sub"
            var sub = User.FindFirstValue("sub");
            if (!string.IsNullOrEmpty(sub) && long.TryParse(sub, out var subId))
                return subId;

            throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong token.");
        }
    }

    /// <summary>Xem giỏ hàng hiện tại</summary>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var result = await _cartService.GetCartAsync(CurrentUserId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Thêm sản phẩm vào giỏ</summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
    {
        try
        {
            var result = await _cartService.AddToCartAsync(CurrentUserId, request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Cập nhật số lượng sản phẩm (quantity = 0 sẽ xóa item)</summary>
    [HttpPut("items")]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var result = await _cartService.UpdateItemAsync(CurrentUserId, request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Xóa 1 sản phẩm khỏi giỏ</summary>
    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(long cartItemId)
    {
        try
        {
            var result = await _cartService.RemoveItemAsync(CurrentUserId, cartItemId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Xóa toàn bộ giỏ hàng</summary>
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            await _cartService.ClearCartAsync(CurrentUserId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}