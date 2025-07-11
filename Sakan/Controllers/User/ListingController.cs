using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using System.Security.Claims;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingController(IListingService listingService)
        {
            _listingService = listingService;
        }


        [HttpPost("{hostId}")]
        public async Task<IActionResult> CreateListing([FromForm] IFormCollection form, [FromRoute] string hostId)
        {
            try
            {

                var dto = await _listingService.ParseCreateListingFormAsync(form);
                await _listingService.CreateListingAsync(dto, hostId);
                return Ok(new { message = "Listing created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = true,
                    message = ex.Message,
                    stack = ex.StackTrace,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
            
                var name = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                return Ok(new { name, role });
            }
            return Unauthorized("User is NOT authenticated");
        }

    }
}
