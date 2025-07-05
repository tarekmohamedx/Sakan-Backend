using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
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
             .Include(r => r.RoomPhotos)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.Listing.HostId == hostId);

        if (room == null) return false;

        // Update room fields
        room.Name = dto.Name;
        room.Type = dto.Type;
        room.PricePerNight = dto.PricePerNight;
        room.MaxGuests = dto.MaxGuests;
        room.IsBookableAsWhole = dto.IsBookableAsWhole;
        room.IsActive = dto.IsActive;

        // --- Sync Room Photos ---
        var existingRoomPhotos = await _context.RoomPhotos.Where(p => p.RoomId == room.Id).ToListAsync();
        var existingRoomPhotoUrls = existingRoomPhotos.Select(p => p.PhotoUrl).ToHashSet();

        // Delete removed photos
        if (dto.RoomPhotoUrls != null && dto.RoomPhotoUrls.Count > 0)
        {
            var removedRoomPhotos = existingRoomPhotos
                .Where(p => !dto.RoomPhotoUrls.Contains(p.PhotoUrl))
                .ToList();
            _context.RoomPhotos.RemoveRange(removedRoomPhotos);
        }

        var newRoomPhotoUrls = dto.RoomPhotoUrls.Where(url => !existingRoomPhotoUrls.Contains(url)).ToList();
        if (newRoomPhotoUrls.Any())
        {
            foreach (var url in newRoomPhotoUrls)
            {
                _context.RoomPhotos.Add(new RoomPhoto
                {
                    RoomId = room.Id,
                    PhotoUrl = url
                });
            }
        }

        // Add new photos

        //foreach (var url in newRoomPhotoUrls)
        //{
        //    _context.RoomPhotos.Add(new RoomPhoto
        //    {
        //        RoomId = room.Id,
        //        PhotoUrl = url
        //    });
        //}

        // --- Sync Beds ---
        var incomingBedIds = dto.Beds.Where(b => b.Id.HasValue).Select(b => b.Id.Value).ToList();

        // Remove beds not in the DTO
        var bedsToRemove = room.Beds.Where(b => !incomingBedIds.Contains(b.Id)).ToList();
        foreach (var bed in bedsToRemove)
        {
            bed.IsDeleted = true;
        }

        foreach (var bedDto in dto.Beds)
        {
            Bed bed;
            if (bedDto.Id.HasValue)
            {
                bed = room.Beds.FirstOrDefault(b => b.Id == bedDto.Id.Value);
                if (bed != null)
                {
                    bed.Label = bedDto.Label;
                    bed.Type = bedDto.Type;
                    bed.Price = bedDto.Price;
                    bed.IsAvailable = bedDto.IsAvailable;
                }
                else continue;
            }
            else
            {
                bed = new Bed
                {
                    Label = bedDto.Label,
                    Type = bedDto.Type,
                    Price = bedDto.Price,
                    IsAvailable = bedDto.IsAvailable
                };
                room.Beds.Add(bed);
                await _context.SaveChangesAsync(); // generate bed.Id
            }

            // --- Sync Bed Photos ---
            var existingBedPhotos = await _context.BedPhotos.Where(p => p.BedId == bed.Id).ToListAsync();

            var existingBedPhotoUrls = existingBedPhotos.Select(p => p.PhotoUrl).ToHashSet();

            var removedBedPhotos = existingBedPhotos.Where(p => !bedDto.BedPhotos.Contains(p.PhotoUrl)).ToList();
            _context.BedPhotos.RemoveRange(removedBedPhotos);

            var newBedPhotoUrls = bedDto.BedPhotos.Where(url => !existingBedPhotoUrls.Contains(url)).ToList();

            foreach (var url in newBedPhotoUrls)
            {
                _context.BedPhotos.Add(new BedPhoto
                {
                    BedId = bed.Id,
                    PhotoUrl = url
                });
            }
        }

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
