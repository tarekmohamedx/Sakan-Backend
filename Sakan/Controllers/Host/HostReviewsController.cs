using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.Host;
using Sakan.Application.Interfaces.Host;
using Sakan.Domain.Interfaces;
using Stripe;
using System.Security.Claims;

namespace Sakan.Controllers.Host
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostReviewsController : ControllerBase
    {
        private readonly IHostReviewsService _reviewService;

        public HostReviewsController(IHostReviewsService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("host-reviews")]
        public async Task<IActionResult> GetHostUserReviews(string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = await _reviewService.GetUserReviewsByHostAsync(hostId);
            return Ok(reviews);
        }

        [HttpPost("review-user")]
        public async Task<IActionResult> ReviewUser(ReviewDto dto, string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.CreateReviewAsync(hostId, dto);
            return result ? Ok() : BadRequest("Failed to submit review");
        }


        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews(string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = await _reviewService.GetMyReviewsAsync(hostId);
            return Ok(reviews);
        }

        [HttpPost("create-review")]
        public async Task<IActionResult> createOrUpdateReview(ReviewDto dto, string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.CreateOrUpdateReviewAsync(hostId, dto);
            return result ? Ok() : BadRequest("Failed to submit review");
        }


    }
}
