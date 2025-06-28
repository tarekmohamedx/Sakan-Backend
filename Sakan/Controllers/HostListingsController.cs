using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using System.Security.Claims;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Customer")]
    public class HostListingsController : ControllerBase
    {
        private readonly IHostListingService _listingService;
        public string hostId = "host-123";

        public HostListingsController(IHostListingService listingService)
        {
            _listingService = listingService;
        }


        [HttpGet("my")]
        public async Task<IActionResult> GetMyListings([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var pagedResult = await _listingService.GetHostListingsPagedAsync(hostId, page, pageSize);
            return Ok(pagedResult);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(int id)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var listing = await _listingService.GetListingByIdAsync(id, hostId);
            if (listing == null)
                return NotFound();

            return Ok(listing);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListing(int id, [FromBody] ListingEditDto updated)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var success = await _listingService.UpdateListingWithPhotosAsync(id, hostId, updated);
            if (!success)
                return NotFound("Listing not found or access denied.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListing(int id)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var success = await _listingService.DeleteListingAsync(id, hostId);
            if (!success)
                return NotFound("Listing not found or access denied.");

            return NoContent();
        }
    }
}
