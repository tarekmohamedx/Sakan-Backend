using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services;

namespace Sakan.Controllers.Host
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostDashboardController : ControllerBase
    {
        public HostDashboardController(IHostDashboardService _hostDashboardService)
        {
            HostDashboardService = _hostDashboardService;
        }

        public IHostDashboardService HostDashboardService { get; }

        [HttpGet("GetRequestedCount/{userId}")]
        public async Task<IActionResult> GetRequestedCount(string userId) {
            var count = await HostDashboardService.GetRequestedCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("{hostId}")]
        public async Task<IActionResult> GetDashboard(string hostId)
        {
            var dashboard = await HostDashboardService.GetDashboardAsync(hostId);

            if (dashboard == null)
                return NotFound();

            return Ok(dashboard);
        }
    }
}
