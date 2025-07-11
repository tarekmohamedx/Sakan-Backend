using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.Admin;
using Sakan.Application.Interfaces.Admin;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;

namespace Sakan.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
    public class AdminHostsApproveController : ControllerBase
    {
        private readonly IAdminHostsService _hostAdminService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminHostsApproveController(IAdminHostsService hostAdminService, UserManager<ApplicationUser> userManager)
        {
            _hostAdminService = hostAdminService;
            _userManager = userManager;
        }

        [HttpGet("hosts")]
        public async Task<IActionResult> GetAllHosts()
        {
            var result = await _hostAdminService.GetAllHostsAsync();
            return Ok(result);
            
        }

        [HttpPost("approve-host")]
        public async Task<IActionResult> ApproveHost([FromBody] AdminHostApprovalDto dto)
        {
            var message = await _hostAdminService.HandleHostApprovalAsync(dto);

            if (message == "User not found")
                return NotFound();

            return Ok(new { message });
        }

    }
}
