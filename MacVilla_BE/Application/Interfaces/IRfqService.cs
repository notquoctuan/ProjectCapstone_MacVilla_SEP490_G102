using Application.DTOs;

namespace Application.Interfaces;

public interface IRfqService
{
    // ── Customer ─────────────────────────────────────────────────────────────

    /// <summary>Khách hàng gửi yêu cầu báo giá mới</summary>
    Task<RfqDetailResponse> CreateRfqAsync(CreateRfqRequest request, long? userId);

    /// <summary>Khách hàng xem danh sách yêu cầu báo giá của mình</summary>
    Task<PagedResponse<RfqListResponse>> GetMyRfqsAsync(long userId, string? status, int pageNumber, int pageSize);

    /// <summary>Khách hàng xem chi tiết 1 yêu cầu (chỉ được xem của mình)</summary>
    Task<RfqDetailResponse?> GetMyRfqDetailAsync(long rfqId, long userId);

    /// <summary>Khách hàng huỷ yêu cầu báo giá (chỉ khi Pending)</summary>
    Task<bool> CancelRfqAsync(long rfqId, long userId);

    // ── Admin / Sale ─────────────────────────────────────────────────────────

    /// <summary>Xem danh sách tất cả RFQ (có filter, paginate)</summary>
    Task<PagedResponse<RfqListResponse>> GetAllRfqsAsync(RfqFilterRequest request);

    /// <summary>Xem chi tiết RFQ bất kỳ</summary>
    Task<RfqDetailResponse?> GetRfqDetailAsync(long rfqId);

    /// <summary>Cập nhật trạng thái RFQ (Pending → Processing → Quoted / Closed / Cancelled)</summary>
    Task<bool> UpdateRfqStatusAsync(long rfqId, UpdateRfqStatusRequest request);

    /// <summary>Gán nhân viên Sale phụ trách RFQ</summary>
    Task<bool> AssignSaleAsync(long rfqId, AssignSaleRequest request);

    /// <summary>Cập nhật ghi chú nội bộ</summary>
    Task<bool> UpdateInternalNoteAsync(long rfqId, UpdateRfqInternalNoteRequest request);
}
