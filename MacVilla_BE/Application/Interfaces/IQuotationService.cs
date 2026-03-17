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
