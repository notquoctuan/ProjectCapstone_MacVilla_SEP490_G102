using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        public DashboardController(DashboardService dashboardService) => _dashboardService = dashboardService;

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(data);
        }
    }
}
