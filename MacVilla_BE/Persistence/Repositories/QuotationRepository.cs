using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class QuotationRepository : IQuotationRepository
{
    private readonly MacvilladbContext _context;

    public QuotationRepository(MacvilladbContext context) => _context = context;

    // ── Admin search ──────────────────────────────────────────────────────────
    public async Task<(IEnumerable<Quotation> Items, int TotalCount)> SearchQuotationsAsync(
        string? status, string? keyword, long? createdBy,
        DateTime? startDate, DateTime? endDate,
        int pageNumber, int pageSize)
    {
        var query = _context.Quotations
            .Include(q => q.Rfq)
            .Include(q => q.CreatedByUser)
            .Include(q => q.QuotationItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(q => q.Status == status);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(q =>
                (q.QuotationCode != null && q.QuotationCode.ToLower().Contains(kw)) ||
                (q.Rfq != null && q.Rfq.CustomerName != null && q.Rfq.CustomerName.ToLower().Contains(kw)) ||
                (q.Rfq != null && q.Rfq.CompanyName != null && q.Rfq.CompanyName.ToLower().Contains(kw)));
        }

        if (createdBy.HasValue)
            query = query.Where(q => q.CreatedBy == createdBy.Value);

        if (startDate.HasValue)
            query = query.Where(q => q.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(q => q.CreatedAt <= endDate.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, total);
    }

    // ── Detail ────────────────────────────────────────────────────────────────
    public async Task<Quotation?> GetQuotationDetailByIdAsync(long quotationId)
        => await _context.Quotations
            .Include(q => q.Rfq)
            .Include(q => q.CreatedByUser)
            .Include(q => q.QuotationItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p!.ProductImages)
            .FirstOrDefaultAsync(q => q.QuotationId == quotationId);

    public async Task<Quotation?> GetByIdAsync(long quotationId)
        => await _context.Quotations
            .Include(q => q.Rfq)
            .FirstOrDefaultAsync(q => q.QuotationId == quotationId);

    // ── Customer queries ──────────────────────────────────────────────────────
    public async Task<(IEnumerable<Quotation> Items, int TotalCount)> GetByRfqUserIdAsync(
        long userId, string? status, int pageNumber, int pageSize)
    {
        var query = _context.Quotations
            .Include(q => q.Rfq)
            .Include(q => q.CreatedByUser)
            .Where(q => q.Rfq != null && q.Rfq.UserId == userId
                        && q.Status != "Draft")  // Khách không thấy bản nháp
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(q => q.Status == status);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, total);
    }

    public async Task<Quotation?> GetByIdAndUserIdAsync(long quotationId, long userId)
        => await _context.Quotations
            .Include(q => q.Rfq)
            .Include(q => q.CreatedByUser)
            .Include(q => q.QuotationItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p!.ProductImages)
            .FirstOrDefaultAsync(q =>
                q.QuotationId == quotationId &&
                q.Rfq != null && q.Rfq.UserId == userId &&
                q.Status != "Draft");

    // ── Write operations ──────────────────────────────────────────────────────
    public async Task<Quotation> CreateQuotationAsync(Quotation quotation)
    {
        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync();
        return quotation;
    }

    public async Task<Quotation> UpdateQuotationAsync(Quotation quotation)
    {
        _context.Quotations.Update(quotation);
        await _context.SaveChangesAsync();
        return quotation;
    }

    // ── Sequence for code generation ──────────────────────────────────────────
    public async Task<int> GetNextSequenceAsync(int year)
    {
        var startOfYear = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfYear = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var count = await _context.Quotations
            .CountAsync(q => q.CreatedAt >= startOfYear && q.CreatedAt < endOfYear);

        return count + 1;
    }
}
