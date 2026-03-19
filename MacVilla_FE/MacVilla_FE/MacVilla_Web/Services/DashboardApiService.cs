using System.Net.Http;

namespace MacVilla_Web.Services
{
    public class DashboardApiService
    {
        private readonly HttpClient _httpClient;
        public DashboardApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7262/");
        }
        public async Task<DashboardVM> GetDashboardSummaryAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardVM>("api/admin/dashboard/summary")
                       ?? new DashboardVM();
            }
            catch
            {
                return new DashboardVM();
            }
        }
    }
}
