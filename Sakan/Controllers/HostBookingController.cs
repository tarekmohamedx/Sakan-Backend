using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces;
using Sakan.Domain.Interfaces;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostBookingController : ControllerBase
    {
        private readonly IHostBookingService _bookingService;

        public HostBookingController(IHostBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("host/bookings")]
        public async Task<IActionResult> GetBookingsForHost(string hostId)
        {
            var bookings = await _bookingService.GetHostBookingsAsync(hostId);
            return Ok(bookings);
        }

    }
}
