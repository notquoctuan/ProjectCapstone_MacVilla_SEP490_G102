using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly MacvilladbContext _context;

        public DashboardRepository(MacvilladbContext context)
        {
            _context = context;
        }

        public async Task<int> CountActiveProductsAsync()
            => await _context.Products.CountAsync(p => p.Status == "Enable");

        public async Task<int> CountNewOrdersTodayAsync()
            => await _context.Orders
                .CountAsync(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == DateTime.Today);

        public async Task<int> CountTotalCustomersAsync()
            => await _context.Users.CountAsync(u => u.Role == "Customer");

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            var now = DateTime.Now;
            return await _context.Orders
                .Where(o => o.CreatedAt.HasValue &&
                            o.CreatedAt.Value.Month == now.Month &&
                            o.CreatedAt.Value.Year == now.Year &&
                            o.Status != "Cancelled")
                .SumAsync(o => o.TotalAmount ?? 0);
        }

        public async Task<List<RevenueChartData>> GetRecentRevenueDataAsync(int days)
        {
            var startDate = DateTime.Today.AddDays(-(days - 1));

            // Bước 1: Lấy dữ liệu thô từ Database
            var rawData = await _context.Orders
                .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= startDate && o.Status != "Cancelled")
                .GroupBy(o => o.CreatedAt!.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Value = g.Sum(o => o.TotalAmount ?? 0)
                })
                .ToListAsync();

            // Bước 2: Format ở client (RAM)
            var formattedData = rawData
                .Select(x => new RevenueChartData
                {
                    Date = x.Date.ToString("dd/MM"),
                    Value = x.Value
                })
                .OrderBy(x => x.Date)
                .ToList();

            return formattedData;
        }
    }
}