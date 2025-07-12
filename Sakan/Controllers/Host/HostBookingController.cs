using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces;
using Sakan.Application.Interfaces.Host;
using Sakan.Infrastructure.Services;

namespace Sakan.Controllers.Host
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

        //Hostbooking/host/bookings
        [HttpGet("host/bookings/{hostId}")]
        public async Task<IActionResult> GetBookingsForHost(string hostId)
        {
            var bookings = await _bookingService.GetHostBookingsAsync(hostId);
            return Ok(bookings);
        }


        }
}
