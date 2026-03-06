using Application.DTOs;
using Domain.Interfaces;
using Domain.Entities; // ⭐ Thêm để nhận diện dữ liệu từ Repository
using BCrypt.Net; // ⭐ Thêm dòng này
namespace Application.Services
{
    public class DashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync()
        {
            // ⭐ Đảm bảo DashboardSummaryResponse trong DTO dùng List<Domain.Entities.RevenueChartData>
            return new DashboardSummaryResponse
            {
                TotalActiveProducts = await _dashboardRepo.CountActiveProductsAsync(),
                NewOrdersToday = await _dashboardRepo.CountNewOrdersTodayAsync(),
                TotalCustomers = await _dashboardRepo.CountTotalCustomersAsync(),
                MonthlyRevenue = await _dashboardRepo.GetMonthlyRevenueAsync(),
                RevenueChart = await _dashboardRepo.GetRecentRevenueDataAsync(7)
            };
        }
    }
}