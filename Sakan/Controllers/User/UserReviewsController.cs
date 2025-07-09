using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserReviewsController : ControllerBase
    {
        private readonly IUserReviewService _reviewService;

        public UserReviewsController(IUserReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReview([FromBody] UserCreateReviewDto dto, string userId)
        {
            var result = await _reviewService.CreateOrUpdateReviewAsync(userId, dto);
            if (result) return Ok();
            return BadRequest();
        }

        [HttpGet("listing/{listingId}")]
        public async Task<IActionResult> GetListingReviews(int listingId)
        {
            var reviews = await _reviewService.GetReviewsByListingIdAsync(listingId);
            return Ok(reviews);
        }

        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetHostReviews(string userId)
        {
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            return Ok(reviews);
        }

        [HttpGet("user-bookings/{userId}")]
        public async Task<IActionResult> GetUserBookings(string userId)
        {
            var bookings = await _reviewService.GetUserBookingsWithReviewStatusAsync(userId);
            return Ok(bookings);
        }


    }
}
