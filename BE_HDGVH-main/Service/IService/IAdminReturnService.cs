using BE_API.Dto.Admin;
using BE_API.Dto.Common;

namespace BE_API.Service.IService;

public interface IAdminReturnService
{
    /// <summary>
    /// Lấy danh sách phiếu đổi/trả có phân trang và filter
    /// </summary>
    Task<PagedResultDto<AdminReturnListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        string? type = null,
        int? customerId = null,
        int? orderId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy chi tiết phiếu đổi/trả theo ID
    /// </summary>
    Task<AdminReturnDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy chi tiết phiếu đổi/trả theo mã phiếu
    /// </summary>
    Task<AdminReturnDetailDto> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo phiếu đổi/trả
    /// </summary>
    Task<AdminReturnDetailDto> CreateAsync(
        AdminReturnCreateDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Manager duyệt phiếu đổi/trả
    /// </summary>
    Task<AdminReturnDetailDto> ApproveAsync(
        int id,
        int managerId,
        AdminReturnApproveDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Manager từ chối phiếu đổi/trả
    /// </summary>
    Task<AdminReturnDetailDto> RejectAsync(
        int id,
        int managerId,
        AdminReturnRejectDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Hoàn thành phiếu đổi/trả (sau khi thu hồi hàng + hoàn tiền)
    /// </summary>
    Task<AdminReturnDetailDto> CompleteAsync(
        int id,
        int stockManagerId,
        AdminReturnCompleteDto dto,
        CancellationToken cancellationToken = default);
}
