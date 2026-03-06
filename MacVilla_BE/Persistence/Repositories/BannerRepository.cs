using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class BannerRepository : IBannerRepository
{
    private readonly MacvilladbContext _context;

    public BannerRepository(MacvilladbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Banner>> GetActiveBannersAsync()
    {
        return await _context.Set<Banner>()
            .AsNoTracking()
            .Where(b => b.IsActive == true)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();
    }
}
