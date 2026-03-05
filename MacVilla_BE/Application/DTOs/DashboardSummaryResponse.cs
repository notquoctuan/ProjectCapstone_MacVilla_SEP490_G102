// File: Application/DTOs/DashboardSummaryResponse.cs
using Domain.Entities; // ⭐ Tham chiếu trực tiếp đến Domain

namespace Application.DTOs
{
    public class DashboardSummaryResponse
    {
        public int TotalActiveProducts { get; set; }
        public int NewOrdersToday { get; set; }
        public int TotalCustomers { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // ⭐ Đảm bảo dùng List từ Domain.Entities
        public List<RevenueChartData> RevenueChart { get; set; } = new();
    }
}