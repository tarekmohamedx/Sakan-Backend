using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenityService _amenityService;

        public AmenitiesController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        // GET: api/amenities
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var amenities = await _amenityService.GetAllAsync();
            return Ok(amenities);
        }
    }
}
