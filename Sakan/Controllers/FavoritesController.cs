using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services_Interfaces;

namespace Sakan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // هذه العمليات تتطلب أن يكون المستخدم مسجلاً دخوله
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // POST: api/favorites/toggle/123  (حيث 123 هو رقم الوحدة)
        [HttpPost("toggle/{listingId:int}")]
        public async Task<IActionResult> ToggleFavorite(int listingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isFavorite = await _favoriteService.ToggleFavoriteAsync(userId, listingId);
            return Ok(new { isFavorite });
        }

        // GET: api/favorites
        [HttpGet]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }
    }
}
