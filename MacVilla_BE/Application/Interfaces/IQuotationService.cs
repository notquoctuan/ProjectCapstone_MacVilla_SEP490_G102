using Application.DTOs;

namespace Application.Interfaces;

public interface IQuotationService
{
    // ── Admin / Sale ─────────────────────────────────────────────────────────

    /// <summary>Tạo báo giá mới từ một RFQ</summary>
    Task<QuotationDetailResponse> CreateQuotationAsync(CreateQuotationRequest request, long createdByUserId);

    /// <summary>Cập nhật nội dung báo giá (chỉ khi Draft)</summary>
    Task<QuotationDetailResponse> UpdateQuotationAsync(long quotationId, UpdateQuotationRequest request);

    /// <summary>Xem danh sách tất cả báo giá (có filter, paginate)</summary>
    Task<PagedResponse<QuotationListResponse>> GetAllQuotationsAsync(QuotationFilterRequest request);

    /// <summary>Xem chi tiết 1 báo giá</summary>
    Task<QuotationDetailResponse?> GetQuotationDetailAsync(long quotationId);

    /// <summary>Gửi báo giá cho khách hàng (chuyển Draft → SentToCustomer)</summary>
    Task<bool> SendQuotationAsync(long quotationId);

    /// <summary>Xuất PDF báo giá</summary>
    Task<byte[]> ExportPdfAsync(long quotationId);

    /// <summary>Thống kê hiệu quả báo giá và yêu cầu (Dashboard)</summary>
    Task<RfqDashboardStatsResponse> GetDashboardStatsAsync();

    /// <summary>Hủy báo giá (Draft/SentToCustomer) thay vì xóa bỏ</summary>
    Task<bool> CancelQuotationAsync(long quotationId);

    /// <summary>Tạo báo giá mới (Clone) từ một báo giá đã tồn tại để điều chỉnh</summary>
    Task<QuotationDetailResponse> ReviseQuotationAsync(long quotationId, long createdByUserId);

    /// <summary>Tạo đơn hàng từ báo giá đã được Approved</summary>
    Task<long> CreateOrderFromQuotationAsync(long quotationId, long createdByUserId);

    // ── Customer ─────────────────────────────────────────────────────────────

    /// <summary>Khách hàng xem danh sách báo giá đã nhận</summary>
    Task<PagedResponse<QuotationListResponse>> GetMyQuotationsAsync(long userId, string? status, int pageNumber, int pageSize);

    /// <summary>Khách hàng xem chi tiết báo giá của mình</summary>
    Task<QuotationDetailResponse?> GetMyQuotationDetailAsync(long quotationId, long userId);

    /// <summary>Khách hàng chấp nhận báo giá (SentToCustomer → Approved)</summary>
    Task<bool> ApproveQuotationAsync(long quotationId, long userId);

    /// <summary>Khách hàng từ chối báo giá (SentToCustomer → Rejected)</summary>
    Task<bool> RejectQuotationAsync(long quotationId, long userId, RejectQuotationRequest request);
}
