using Domain.Entities;

namespace Domain.Interfaces;

public interface IQuotationRepository
{
    // ── Admin / Sale ─────────────────────────────────────────────────────────

    Task<(IEnumerable<Quotation> Items, int TotalCount)> SearchQuotationsAsync(
        string? status,
        string? keyword,
        long? createdBy,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize);

    Task<Quotation?> GetQuotationDetailByIdAsync(long quotationId);

    Task<Quotation?> GetByIdAsync(long quotationId);

    Task<Quotation> CreateQuotationAsync(Quotation quotation);

    Task<Quotation> UpdateQuotationAsync(Quotation quotation);

    // ── Customer ─────────────────────────────────────────────────────────────

    Task<(IEnumerable<Quotation> Items, int TotalCount)> GetByRfqUserIdAsync(
        long userId,
        string? status,
        int pageNumber,
        int pageSize);

    Task<Quotation?> GetByIdAndUserIdAsync(long quotationId, long userId);

    // ── Shared ───────────────────────────────────────────────────────────────

    Task<int> GetNextSequenceAsync(int year);
}
