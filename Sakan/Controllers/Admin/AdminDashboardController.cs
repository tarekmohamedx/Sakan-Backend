using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces.Admin;

namespace Sakan.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
         
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary() => Ok(await _dashboardService.GetSummaryAsync());

        [HttpGet("activity")]
        public async Task<IActionResult> GetRecentActivity() => Ok(await _dashboardService.GetRecentActivityAsync());

    }
}
