using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using System.Security.Claims;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Host")] 
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDTO dto)
        {
            try
            {
                // Get host ID from JWT token (authenticated user)
                var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(hostId))
                    return Unauthorized("Host ID not found in token.");

                await _reviewService.AddReviewAsync(dto, hostId);

                return Ok(new { message = "Review submitted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
