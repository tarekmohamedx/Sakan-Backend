using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingRequestController : ControllerBase
    {
        private readonly IBookingRequestService _bookingRequestService;

        public BookingRequestController(IBookingRequestService bookingRequestService)
        {
            _bookingRequestService = bookingRequestService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingRequestsByUserId(String userId)
        {
            var requests = await _bookingRequestService.GetBookingRequestsByUserIdAsync(userId);
            return Ok(requests);
        }

        [HttpPost("update/{requestId}/{isAccepted}")]
        public async Task<IActionResult> UpdateBookingRequest(int requestId, bool isAccepted)
        {
            var result = await _bookingRequestService.UpdateBookingRequestAsync(requestId, isAccepted);
            if (result)
            {
                return Ok(new { message = "Booking request updated successfully." });
            }
            return BadRequest(new { message = "Failed to update booking request." });
        }

        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetBookingRequestsByHostId(string hostId)
        {
            var requests = await _bookingRequestService.GetBookingRequestsByHostIdAsync(hostId);
            return Ok(requests);
        }
    }
}
