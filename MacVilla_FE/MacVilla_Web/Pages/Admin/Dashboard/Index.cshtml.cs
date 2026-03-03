using MacVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MacVilla_Web.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly DashboardApiService _apiService; // Giả sử bạn dùng chung Service này
        public IndexModel(DashboardApiService apiService) => _apiService = apiService;

        public DashboardVM Data { get; set; } = new();

        public async Task OnGetAsync()
        {
            Data = await _apiService.GetDashboardSummaryAsync();
        }
    }
}
