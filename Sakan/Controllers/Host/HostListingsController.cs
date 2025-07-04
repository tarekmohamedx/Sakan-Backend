using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using System.Security.Claims;

namespace Sakan.Controllers.Host
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostListingsController : ControllerBase
    {
        private readonly IHostListingService _listingService;

        public HostListingsController(IHostListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyListings(string hostId,[FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var listings = await _listingService.GetHostListingsAsync(hostId, page, pageSize, search);

            return Ok(listings);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(int id, string hostId)
        {
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var listing = await _listingService.GetListingByIdAsync(id, hostId);
            if (listing == null)
                return NotFound();

            return Ok(listing);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListing(int id, string hostId, [FromBody] ListingEditDto updated)
        {
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var success = await _listingService.UpdateListingWithPhotosAsync(id, hostId, updated);
            if (!success)
                return NotFound("Listing not found or access denied.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListing(int id, string hostId)
        {
            if (string.IsNullOrEmpty(hostId))
                return Unauthorized("Host ID not found in token.");

            var success = await _listingService.DeleteListingAsync(id, hostId);
            if (!success)
                return NotFound("Listing not found or access denied.");

            return NoContent();
        }
    }
}
