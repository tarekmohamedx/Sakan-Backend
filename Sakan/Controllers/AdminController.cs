using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminListingService _adminListingService;

        public AdminController(IAdminListingService adminListingService)
        {
            _adminListingService = adminListingService;
        }

        [HttpGet("not-approved-listings")]
        public async Task<IActionResult> GetNotApprovedListings([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _adminListingService.GetNotApprovedListingsAsync(pageNumber, pageSize /*, search */);
            return Ok(new
            {
                items = result.Items,
                totalCount = result.TotalCount
            });
        }
        [HttpPost("approve/{listingId}")]
        public async Task<IActionResult> ApproveListing(int listingId)
        {
            var result = await _adminListingService.ApproveListingAsync(listingId);
            return result ? Ok("Listing approved") : BadRequest("Approval failed");
        }

        [HttpPost("reject/{listingId}")]
        public async Task<IActionResult> RejectListing(int listingId)
        {
            var result = await _adminListingService.RejectListingAsync(listingId);
            return result ? Ok("Listing rejected") : BadRequest("Rejection failed");
        }
    }
}
