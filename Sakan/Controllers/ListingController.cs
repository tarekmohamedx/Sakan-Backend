using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using System.Security.Claims;

namespace Sakan.Controllers
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

      //  [Authorize(Roles = "Host")]  this filter will decode token after send it from client 
        [HttpPost]
        public async Task<IActionResult> CreateListing([FromForm] CreateListingDTO dto ,[FromRoute]string hostId)
        {
            try
            {
               // var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var token = Request.Headers["Authorization"];
                await _listingService.CreateListingAsync(dto, hostId);
                return Ok(new { message = $"Listing created successfully.  + token = {token}" });
            }
            catch (Exception ex)
            {
                // Log the error if needed
                return BadRequest(new
                {
                    error = true,
                    message = ex.Message
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
