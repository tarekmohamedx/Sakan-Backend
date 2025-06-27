using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Infrastructure.Models;

public class RoomDetailsService : IRoomDetailsService
{
    private readonly sakanContext _context;

    public RoomDetailsService(sakanContext context)
    {
        _context = context;
    }

    public async Task<RoomDto> GetRoomDetailsAsync(int roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.RoomPhotos)
            .Include(r => r.Beds).ThenInclude(b => b.BedPhotos)
            .Include(r => r.Listing)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null) return null;

        return new RoomDto
        {
            Id = room.Id,
            ListingId = room.ListingId,
            Name = room.Name,
            PricePerNight = room.PricePerNight,
            IsBookableAsWhole = room.IsBookableAsWhole,
            Photos = room.RoomPhotos.Select(p => p.PhotoUrl).ToList(),
            Beds = room.Beds.Select(b => new BedDto
            {
                Id = b.Id,
                Label = b.Label,
                Type = b.Type,
                Price = b.Price,
                IsAvailable = b.IsAvailable,
                BedPhotos = b.BedPhotos.Select(p => p.PhotoUrl).ToList()
            }).ToList(),
            Listing = new ListingLocationDto
            {
                Latitude = room.Listing.Latitude ?? 0,
                Longitude = room.Listing.Longitude ?? 0
            }
        };
    }
}
