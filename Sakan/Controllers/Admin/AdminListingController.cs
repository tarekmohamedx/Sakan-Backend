using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.Host;
using Sakan.Application.Interfaces.Admin;
using Stripe;

namespace Sakan.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/list")]
    public class AdminListingController : ControllerBase
    {
        private readonly IAdminListingService _listingService;
        public AdminListingController(IAdminListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetMyListings([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var listings = await _listingService.GetAlltListingsAsync(page, pageSize, search);

            return Ok(listings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(int id)
        {
            var listing = await _listingService.GetListingByIdAsync(id);
            if (listing == null)
                return NotFound();

            return Ok(listing);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListing(int id, [FromBody] ListingEditDto updated)
        {
            var success = await _listingService.UpdateListingWithPhotosAsync(id, updated);
            if (!success)
                return NotFound("Listing not found or access denied.");

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListing(int id)
        {
            var success = await _listingService.DeleteListingAsync(id);
            if (!success)
                return NotFound("Listing not found or access denied.");

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveListing(int id)
        {
            var result = await _listingService.SetListingApprovalStatusAsync(id, true);
            return result ? Ok() : NotFound();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectListing(int id)
        {
            var result = await _listingService.SetListingApprovalStatusAsync(id, false);
            return result ? Ok() : NotFound();
        }


    }
}
