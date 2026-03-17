using Domain.Entities;

namespace Domain.Interfaces;

public interface IRfqRepository
{
    // ── Admin / Sale ─────────────────────────────────────────────────────────

    Task<(IEnumerable<Rfq> Items, int TotalCount)> SearchRfqsAsync(
        string? status,
        string? keyword,
        long? assignedSaleId,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize);

    Task<Rfq?> GetRfqDetailByIdAsync(long rfqId);

    Task<Rfq> UpdateRfqAsync(Rfq rfq);

    // ── Customer ─────────────────────────────────────────────────────────────

    Task<(IEnumerable<Rfq> Items, int TotalCount)> GetByUserIdAsync(
        long userId,
        string? status,
        int pageNumber,
        int pageSize);

    Task<Rfq?> GetByIdAndUserIdAsync(long rfqId, long userId);

    // ── Shared ───────────────────────────────────────────────────────────────

    Task<Rfq> CreateRfqAsync(Rfq rfq);

    Task<int> GetNextSequenceAsync(int year);
}
