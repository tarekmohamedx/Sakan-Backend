using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services_Interfaces;
using Sakan.Domain.Common;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingsController(IListingService listingService)
        {
            _listingService = listingService;
        }

        // --- Endpoints لصفحة البحث والفلترة ---
        [HttpGet] // GET /api/listings?pageNumber=2
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
        {
            return Ok(await _listingService.GetAllListingsAsync(pageNumber, pageSize));
        }

        [HttpGet("filter")] // GET /api/listings/filter?minPrice=1000
        public async Task<IActionResult> GetFiltered([FromQuery] ListingFilterParameters filterParams)
        {
            return Ok(await _listingService.GetFilteredListingsAsync(filterParams));
        }

        // --- Endpoints خاصة بالصفحة الرئيسية ---
        [HttpGet("homepage/highest-rated")] // GET /api/listings/homepage/highest-rated
        public async Task<IActionResult> GetHighestRated()
        {
            return Ok(await _listingService.GetHighestRatedListingsAsync(6)); //  مثلاً 6 عناصر
        }

        [HttpGet("homepage/newest")] // GET /api/listings/homepage/newest
        public async Task<IActionResult> GetNewest()
        {
            return Ok(await _listingService.GetNewestListingsAsync(6));
        }

        [HttpGet("homepage/most-affordable")] // GET /api/listings/homepage/most-affordable
        public async Task<IActionResult> GetMostAffordable()
        {
            return Ok(await _listingService.GetMostAffordableListingsAsync(6));
        }
    }
}
