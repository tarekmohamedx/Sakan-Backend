using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces;

namespace Sakan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] 
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/payments/create-intent
        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = "7119cef3-32a8-4b0c-a623-693a9fcc5bc3";
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var clientSecret = await _paymentService.CreatePaymentIntentAsync(request.BookingId, userId);
                return Ok(new { clientSecret });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }

    public class CreatePaymentIntentRequest
    {
        public int BookingId { get; set; }
    }
}
