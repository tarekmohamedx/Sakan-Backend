using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Models;
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

    // for host page 
    public async Task<(List<RoomDetailsDto> Rooms, int TotalCount)> GetRoomsByListingIdAsync(int listingId, string hostId, int page, int pageSize, string? search)
    {
        var query = _context.Rooms
            .Include(r => r.Listing)
            .Include(r => r.Beds).ThenInclude(b => b.BedPhotos)
            .Include(r => r.RoomPhotos)
            .Where(r => r.ListingId == listingId && r.Listing.HostId == hostId && !r.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var rooms = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (
            rooms.Select(room => new RoomDetailsDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type,
                PricePerNight = room.PricePerNight,
                MaxGuests = room.MaxGuests,
                IsBookableAsWhole = room.IsBookableAsWhole,
                IsActive = room.IsActive,
                ListingId = room.ListingId,
                ListingTitle = room.Listing?.Title,
                Beds = room.Beds
                .Where(b => !b.IsDeleted)
                .Select(b => new BedDto
                {
                    Id = b.Id,
                    Label = b.Label,
                    Type = b.Type,
                    Price = b.Price,
                    IsAvailable = b.IsAvailable,
                    BedPhotos = b.BedPhotos.Select(p => p.PhotoUrl).ToList()
                }).ToList(),
                PhotoUrls = room.RoomPhotos.Select(p => p.PhotoUrl).ToList()
            }).ToList(),
            totalCount
        );
    }

    public async Task<RoomDetailsDto?> GetRoomByIdAsync(int roomId, string hostId)
    {
        var room = await _context.Rooms
            .Include(r => r.Listing)
            .Include(r => r.Beds).ThenInclude(b => b.BedPhotos)
            .Include(r => r.RoomPhotos)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.Listing.HostId == hostId);

        if (room == null) return null;

        return new RoomDetailsDto
        {
            Id = room.Id,
            Name = room.Name,
            Type = room.Type,
            PricePerNight = room.PricePerNight,
            MaxGuests = room.MaxGuests,
            IsBookableAsWhole = room.IsBookableAsWhole,
            IsActive = room.IsActive,
            ListingId = room.ListingId,
            ListingTitle = room.Listing?.Title,
            Beds = room.Beds.Select(b => new BedDto
            {
                Id = b.Id,
                Label = b.Label,
                Type = b.Type,
                Price = b.Price,
                IsAvailable = b.IsAvailable,
                BedPhotos = b.BedPhotos.Select(p => p.PhotoUrl).ToList()
            }).ToList(),
            PhotoUrls = room.RoomPhotos.Select(p => p.PhotoUrl).ToList()
        };
    }

    //public async Task<bool> UpdateRoomAsync(int roomId, RoomUpdateDto dto, string hostId)
    //{
    //    var room = await _context.Rooms
    //        .Include(r => r.Listing)
    //        .FirstOrDefaultAsync(r => r.Id == roomId && r.Listing.HostId == hostId);

    //    if (room == null) return false;

    //    room.Name = dto.Name;
    //    room.Type = dto.Type;
    //    room.PricePerNight = dto.PricePerNight;
    //    room.MaxGuests = dto.MaxGuests;
    //    room.IsBookableAsWhole = dto.IsBookableAsWhole;
    //    room.IsActive = dto.IsActive;

    //    _context.Rooms.Update(room);
    //    await _context.SaveChangesAsync();
    //    return true;
    //}

    public async Task<bool> UpdateRoomAsync(int roomId, RoomUpdateDto dto, string hostId)
    {
        var room = await _context.Rooms
            .Include(r => r.Beds)
            .Include(r => r.Listing)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.Listing.HostId == hostId);

        if (room == null) return false;

        // Update room fields
        room.Name = dto.Name;
        room.Type = dto.Type;
        room.PricePerNight = dto.PricePerNight;
        room.MaxGuests = dto.MaxGuests;
        room.IsBookableAsWhole = dto.IsBookableAsWhole;
        room.IsActive = dto.IsActive;

        // Process beds
        var incomingBedIds = dto.Beds
            .Where(b => b.Id.HasValue)
            .Select(b => b.Id.Value)
            .ToList();

        // Remove beds that are no longer included
        var bedsToRemove = room.Beds
            .Where(b => !incomingBedIds.Contains(b.Id))
            .ToList();

        foreach (var bed in bedsToRemove)
        {
            bed.IsDeleted = true;
        }


        // Update or add beds
        foreach (var bedDto in dto.Beds)
        {
            if (bedDto.Id.HasValue)
            {
                // Update existing bed
                var bed = room.Beds.FirstOrDefault(b => b.Id == bedDto.Id.Value);
                if (bed != null)
                {
                    bed.Label = bedDto.Label;
                    bed.Type = bedDto.Type;
                    bed.Price = bedDto.Price;
                    bed.IsAvailable = bedDto.IsAvailable;
                }
            }
            else
            {
                // Add new bed
                room.Beds.Add(new Bed
                {
                    Label = bedDto.Label,
                    Type = bedDto.Type,
                    Price = bedDto.Price,
                    IsAvailable = bedDto.IsAvailable
                });
            }
        }

        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<bool> DeleteRoomAsync(int roomId, string hostId)
    {
        var room = await _context.Rooms
            .Include(r => r.Beds)
            .Include(r => r.Listing)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.Listing.HostId == hostId);

        if (room == null) return false;

        room.IsDeleted = true;

        foreach (var bed in room.Beds)
        {
            bed.IsDeleted = true;
        }

        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<(List<RoomDetailsDto> Rooms, int TotalCount)> GetAllRoomsAsync(string hostId, int page, int pageSize, string? search)
    {
        var query = _context.Rooms
            .Include(r => r.Listing)
            .Include(r => r.Beds).ThenInclude(b => b.BedPhotos)
            .Include(r => r.RoomPhotos)
            .Where(r => r.Listing.HostId == hostId && r.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var rooms = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (
            rooms.Select(room => new RoomDetailsDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type,
                PricePerNight = room.PricePerNight,
                MaxGuests = room.MaxGuests,
                IsBookableAsWhole = room.IsBookableAsWhole,
                IsActive = room.IsActive,
                ListingId = room.ListingId,
                ListingTitle = room.Listing?.Title,
                Beds = room.Beds.Select(b => new BedDto
                {
                    Id = b.Id,
                    Label = b.Label,
                    Type = b.Type,
                    Price = b.Price,
                    IsAvailable = b.IsAvailable,
                    BedPhotos = b.BedPhotos.Select(p => p.PhotoUrl).ToList()
                }).ToList(),
                PhotoUrls = room.RoomPhotos.Select(p => p.PhotoUrl).ToList()
            }).ToList(),
            totalCount
        );
    }



}
