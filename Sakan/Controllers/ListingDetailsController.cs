using Microsoft.AspNetCore.Mvc;
using Sakan.Infrastructure.Services; // make sure this matches your namespace
using Sakan.Application.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingDetailsController : ControllerBase
    {
        private readonly IListingDetailsService _listingService;
        private readonly IBookingRequestService _bookingService;

        public ListingDetailsController(IListingDetailsService listingService, IBookingRequestService bookingService)
        {
            _listingService = listingService;
            _bookingService = bookingService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(int id)
        {
            var listing = await _listingService.GetListingDetails(id);
            if (listing == null) return NotFound();
            return Ok(listing);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetListingById(int id, [FromQuery] string lang = "en")
        //{
        //    var listing = await _listingService.GetListingDetails(id);
        //    if (listing == null) return NotFound();

        //    var translator = new TranslationService();

        //    listing.Title = await translator.TranslateTextAsync(listing.Title, lang);
        //    listing.Description = await translator.TranslateTextAsync(listing.Description, lang);
        //    listing.Location = await translator.TranslateTextAsync(listing.Location, lang);

        //    // Translate bedrooms
        //    if (listing.BedroomList != null)
        //    {
        //        foreach (var room in listing.BedroomList)
        //        {
        //            room.Name = await translator.TranslateTextAsync(room.Name, lang);
        //        }
        //    }

        //    // Translate host fields if needed
        //    if (listing.Host?.Languages != null)
        //    {
        //        for (int i = 0; i < listing.Host.Languages.Count; i++)
        //        {
        //            listing.Host.Languages[i] = await translator.TranslateTextAsync(listing.Host.Languages[i], lang);
        //        }
        //    }


        //    return Ok(listing);
        //}



        [HttpGet("booked-months/{listingId}")]
        public async Task<IActionResult> GetBookedMonths(int listingId, [FromQuery] string lang = "en")
        {
            var result = await _listingService.GetBookedMonthsAsync(listingId);
            return Ok(result);
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreateBookingRequest([FromBody] BookingRequestsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // <-- This returns validation errors
            }
            var result = await _bookingService.CreateAsync(dto);
            return Ok(new { requestId = result.requestId, hostId = result.hostId });
        }

        //[HttpGet("translate")]
        //public async Task<IActionResult> TranslateSample(string text, string lang = "ar")
        //{
        //    var service = new TranslationService();
        //    var translated = await service.TranslateTextAsync(text, lang);
        //    return Ok(new { Original = text, Translated = translated });
        //}

    }
}
