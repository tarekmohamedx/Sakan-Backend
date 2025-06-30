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

        [HttpPost]
        public async Task<IActionResult> CreateListing([FromForm] CreateListingDTO dto)
        {
            try
            {
                var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _listingService.CreateListingAsync(dto, hostId);
                return Ok(new { message = "Listing created successfully." });
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

    }
}
