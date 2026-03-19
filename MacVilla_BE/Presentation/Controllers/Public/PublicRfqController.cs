using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Public;

/// <summary>
/// Khách hàng (khách vãng lai) gửi yêu cầu báo giá mà không cần đăng nhập.
/// </summary>
[ApiController]
[Route("api/public/rfq")]
[AllowAnonymous]
public class PublicRfqController : ControllerBase
{
    private readonly IRfqService _rfqService;

    public PublicRfqController(IRfqService rfqService)
        => _rfqService = rfqService;

    /// <summary>Khách hàng (Guest) gửi yêu cầu báo giá mới</summary>
    /// <remarks>POST: /api/public/rfq</remarks>
    [HttpPost]
    public async Task<ActionResult<RfqDetailResponse>> Create([FromBody] CreateRfqRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            // userId = null nghĩa là khách chưa đăng nhập / không có tài khoản
            var result = await _rfqService.CreateRfqAsync(request, null);
            return Ok(new { message = "Gửi yêu cầu thành công, nhân viên sẽ liên hệ lại sớm nhất qua thông tin bạn cung cấp.", data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi tạo yêu cầu báo giá.", error = ex.Message });
        }
    }
}
