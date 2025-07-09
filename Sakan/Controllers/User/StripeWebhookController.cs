using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces.User;
using Stripe;

namespace Sakan.Controllers.User
{
    [Route("api/stripe-webhook")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(IPaymentService paymentService, ILogger<StripeWebhookController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                await _paymentService.HandleWebhookEventAsync(json, stripeSignature);
                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe webhook signature validation failed.");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing webhook.");
                return StatusCode(500);
            }
        }
    }
}
