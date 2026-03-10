using MacVilla_Web.Models;
using System.Net.Http.Headers;

namespace MacVilla_Web.Services
{
    public class DashboardApiService
    {
        private readonly HttpClient _httpClient;

        public DashboardApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // BE endpoint: GET /api/admin/dashboard
        public async Task<DashboardVM?> GetDashboardSummaryAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardVM>("api/admin/dashboard");
            }
            catch { return null; }
        }
    }
}
