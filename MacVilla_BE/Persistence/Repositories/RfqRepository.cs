using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class RfqRepository : IRfqRepository
{
    private readonly MacvilladbContext _context;

    public RfqRepository(MacvilladbContext context) => _context = context;

    // ── Admin search ──────────────────────────────────────────────────────────
    public async Task<(IEnumerable<Rfq> Items, int TotalCount)> SearchRfqsAsync(
        string? status, string? keyword, long? assignedSaleId,
        DateTime? startDate, DateTime? endDate,
        int pageNumber, int pageSize)
    {
        var query = _context.Rfqs
            .Include(r => r.User)
            .Include(r => r.AssignedSale)
            .Include(r => r.RfqItems)
            .Include(r => r.Quotations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(r =>
                (r.RfqCode != null && r.RfqCode.ToLower().Contains(kw)) ||
                (r.CustomerName != null && r.CustomerName.ToLower().Contains(kw)) ||
                (r.CompanyName != null && r.CompanyName.ToLower().Contains(kw)) ||
                (r.Email != null && r.Email.ToLower().Contains(kw)) ||
                (r.Phone != null && r.Phone.Contains(kw)));
        }

        if (assignedSaleId.HasValue)
            query = query.Where(r => r.AssignedSaleId == assignedSaleId.Value);

        if (startDate.HasValue)
            query = query.Where(r => r.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.CreatedAt <= endDate.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, total);
    }

    // ── Detail ────────────────────────────────────────────────────────────────
    public async Task<Rfq?> GetRfqDetailByIdAsync(long rfqId)
        => await _context.Rfqs
            .Include(r => r.User)
            .Include(r => r.AssignedSale)
            .Include(r => r.RfqItems).ThenInclude(i => i.Product).ThenInclude(p => p!.ProductImages)
            .Include(r => r.Quotations)
            .FirstOrDefaultAsync(r => r.RfqId == rfqId);

    // ── Customer queries ──────────────────────────────────────────────────────
    public async Task<(IEnumerable<Rfq> Items, int TotalCount)> GetByUserIdAsync(
        long userId, string? status, int pageNumber, int pageSize)
    {
        var query = _context.Rfqs
            .Include(r => r.RfqItems)
            .Include(r => r.Quotations)
            .Where(r => r.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, total);
    }

    public async Task<Rfq?> GetByIdAndUserIdAsync(long rfqId, long userId)
        => await _context.Rfqs
            .Include(r => r.RfqItems).ThenInclude(i => i.Product).ThenInclude(p => p!.ProductImages)
            .Include(r => r.Quotations)
            .FirstOrDefaultAsync(r => r.RfqId == rfqId && r.UserId == userId);

    // ── Write operations ──────────────────────────────────────────────────────
    public async Task<Rfq> CreateRfqAsync(Rfq rfq)
    {
        _context.Rfqs.Add(rfq);
        await _context.SaveChangesAsync();
        return rfq;
    }

    public async Task<Rfq> UpdateRfqAsync(Rfq rfq)
    {
        _context.Rfqs.Update(rfq);
        await _context.SaveChangesAsync();
        return rfq;
    }

    // ── Sequence for code generation ──────────────────────────────────────────
    public async Task<int> GetNextSequenceAsync(int year)
    {
        var startOfYear = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfYear = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var count = await _context.Rfqs
            .CountAsync(r => r.CreatedAt >= startOfYear && r.CreatedAt < endOfYear);

        return count + 1;
    }
}
