using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using Sakan.Application.Services;
using System.Security.Claims;

namespace Sakan.Controllers.Host
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Host")]
    public class HostRoomsController : ControllerBase
    {
        private readonly IRoomDetailsService _roomService;

        public HostRoomsController(IRoomDetailsService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("listing/{listingId}")]
        public async Task<IActionResult> GetRoomsByListing( int listingId, string hostId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
        {
            var (rooms, totalCount) = await _roomService.GetRoomsByListingIdAsync(listingId, hostId, page, pageSize, search);

            return Ok(new
            {
                rooms,
                totalCount
            });
        }


        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoom(int roomId, string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var room = await _roomService.GetRoomByIdAsync(roomId, hostId);

            if (room == null)
                return NotFound();

            return Ok(room);
        }


        [HttpPut("{roomId}")]
        public async Task<IActionResult> UpdateRoom(int roomId, [FromBody] RoomUpdateDto dto, string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.UpdateRoomAsync(roomId, dto, hostId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom(int roomId, string hostId)
        {
            //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.DeleteRoomAsync(roomId, hostId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("allRooms")]
        public async Task<IActionResult> GetAllRooms(string hostId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string? search = null)
        {
            var (rooms, totalCount) = await _roomService.GetAllRoomsAsync(hostId, page, pageSize, search);

            return Ok(new
            {
                rooms,
                totalCount
            });
        }
    }
}
