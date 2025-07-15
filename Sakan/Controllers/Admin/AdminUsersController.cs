using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.Admin;
using Sakan.Application.Interfaces.Admin;

namespace Sakan.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUsersService _adminUsersService;

        public AdminUsersController(IAdminUsersService adminUsersService)
        {
            _adminUsersService = adminUsersService;
        }

        [HttpGet("hosts")]
        public async Task<IActionResult> GetAllHosts()
        {
            var result = await _adminUsersService.GetAllHostsAsync();
            return Ok(result);
        }

        [HttpGet("guests")]
        public async Task<IActionResult> GetAllGuests()
        {
            var result = await _adminUsersService.GetAllGuestsAsync();
            return Ok(result);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] GuestAdminViewDto dto)
        {
            var success = await _adminUsersService.UpdateUserAsync(userId, dto.UserName, dto.Email, dto.PhoneNumber);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var success = await _adminUsersService.DeleteUserAsync(userId);
            if (!success) return NotFound();
            return Ok();
        }
    }

}
