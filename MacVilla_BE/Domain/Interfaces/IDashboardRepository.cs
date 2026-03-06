using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> CountActiveProductsAsync();
        Task<int> CountNewOrdersTodayAsync();
        Task<int> CountTotalCustomersAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<List<RevenueChartData>> GetRecentRevenueDataAsync(int days);
    }
}
