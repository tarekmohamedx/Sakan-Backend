using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sakan.Infrastructure.Models;
using System.Security.Claims;
using static Sakan.Controllers.AiController;

namespace Sakan.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class BecomeHostController : ControllerBase
    {
        private readonly sakanContext _context;

        public BecomeHostController(sakanContext context)
        {
            _context = context;
        }

        [HttpPost("become-host")]
        public async Task<IActionResult> BecomeHost([FromBody] BecomeHostRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null) return NotFound();

            user.HostVerificationStatus = "pending";
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("host-status/{userId}")]
        public async Task<IActionResult> GetHostStatus(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();
            return Ok(new { status = user.HostVerificationStatus });
        }


    }

    public class BecomeHostRequest
    {
        public string UserId { get; set; }
    }

}
