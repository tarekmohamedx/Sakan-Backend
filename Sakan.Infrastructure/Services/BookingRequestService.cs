//using Microsoft.EntityFrameworkCore;
//using Sakan.Domain.Models;

//public async Task<(int requestId, string hostId)> CreateAsync(BookingRequestsDto dto)
//{
//    int firstRequestId = 0;

//    foreach (var bedId in dto.BedIds)
//    {
//        var booking = new BookingRequest
//        {
//            GuestId = dto.GuestId,
//            ListingId = dto.ListingId ?? 0,
//            RoomId = dto.RoomId,
//            BedId = bedId,
//            FromDate = dto.FromDate,
//            ToDate = dto.ToDate,
//            HostApproved = false,
//            GuestApproved = false
//        };

//        _context.BookingRequests.Add(booking);
//        await _context.SaveChangesAsync();

//        if (firstRequestId == 0)
//            firstRequestId = booking.Id;
//    }

//    string hostId = await _context.Listings
//        .Where(l => l.Id == dto.ListingId)
//        .Select(l => l.HostId)
//        .FirstOrDefaultAsync();

//    return (firstRequestId, hostId);
//}