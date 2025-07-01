using Sakan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces
{
    public interface IRoomDetailsService
    {
        Task<RoomDto> GetRoomDetailsAsync(int roomId);

        // for host page
        Task<(List<RoomDetailsDto> Rooms, int TotalCount)> GetRoomsByListingIdAsync(int listingId, string hostId, int page, int pageSize, string? search);
        Task<RoomDetailsDto?> GetRoomByIdAsync(int roomId, string hostId);
        Task<bool> UpdateRoomAsync(int roomId, RoomUpdateDto dto, string hostId);
        Task<bool> DeleteRoomAsync(int roomId, string hostId);
        Task<(List<RoomDetailsDto> Rooms, int TotalCount)> GetAllRoomsAsync(string hostId, int page, int pageSize, string? search);



    }
}
