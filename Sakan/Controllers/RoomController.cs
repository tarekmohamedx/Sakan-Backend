using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomDetailsService _roomService;

        public RoomController(IRoomDetailsService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomDetails(int roomId)
        {
            var details = await _roomService.GetRoomDetailsAsync(roomId);
            return details == null ? NotFound() : Ok(details);
        }

    }
}
