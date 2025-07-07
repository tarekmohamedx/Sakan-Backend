using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.Host;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.User
{
    // BookingRequestService.cs
    public class BookingRequestService : IBookingRequestService
    {
        private readonly sakanContext _context;

        public BookingRequestService(sakanContext context)
        {
            _context = context;
        }

        public async Task<(int requestId, string hostId)> CreateAsync(BookingRequestsDto dto)
        {
            int firstRequestId = 0;

            if (dto.BedIds == null || !dto.BedIds.Any())
            {
                // Create a single booking request for the entire room (or listing)
                var booking = new BookingRequest
                {
                    GuestId = dto.GuestId,
                    ListingId = dto.ListingId ?? 0,
                    RoomId = dto.RoomId,
                    BedId = null, // No specific bed
                    FromDate = dto.FromDate,
                    ToDate = dto.ToDate,
                    HostApproved = false,
                    GuestApproved = false
                };

                _context.BookingRequests.Add(booking);
                await _context.SaveChangesAsync();

                firstRequestId = booking.Id;
            }
            else
            {
                foreach (var bedId in dto.BedIds)
                {
                    var booking = new BookingRequest
                    {
                        GuestId = dto.GuestId,
                        ListingId = dto.ListingId ?? 0,
                        RoomId = dto.RoomId,
                        BedId = bedId,
                        FromDate = dto.FromDate,
                        ToDate = dto.ToDate,
                        HostApproved = false,
                        GuestApproved = false
                    };

                    _context.BookingRequests.Add(booking);
                    await _context.SaveChangesAsync();

                    if (firstRequestId == 0)
                        firstRequestId = booking.Id;
                }
            }

            var hostId = await _context.Listings
                .Where(l => l.Id == dto.ListingId)
                .Select(l => l.HostId)
                .FirstOrDefaultAsync();

            return (firstRequestId, hostId);
        }


        //use the BookingRequestsDTO then return all the booking requests for the user with the given userId
        public async Task<IEnumerable<BookingRequestsDTO>> GetBookingRequestsByUserIdAsync(string userId)
        {
            var requests = await _context.BookingRequests
                .Where(br => br.GuestId == userId)
                .Select(br => new BookingRequestsDTO
                {
                    BookingRequestId = br.Id,
                    ListingTitle = _context.Listings
                        .Where(l => l.Id == br.ListingId)
                        .Select(l => l.Title)
                        .FirstOrDefault(),
                    BedPrice = _context.Beds
                        .Where(b => b.Id == br.BedId)
                        .Select(b => b.Price)
                        .FirstOrDefault(),
                    ListingLocation = _context.Listings
                        .Where(l => l.Id == br.ListingId)
                        .Select(l => l.Governorate + " - " + l.District)
                        .FirstOrDefault(),
                    FromDate = (DateTime)br.FromDate,
                    ToDate = (DateTime)br.ToDate
                })
                .ToListAsync();

            return requests;
        }

        public async Task<bool> UpdateBookingRequestAsync(int requestId, bool isAccepted)
        {
            var bookingRequest = await _context.BookingRequests.FindAsync(requestId);
            if (bookingRequest == null) return false;
            bookingRequest.HostApproved = isAccepted;
            if (bookingRequest.GuestApproved == true && isAccepted)
                bookingRequest.IsActive = true;
            else if (bookingRequest.GuestApproved == false && !isAccepted)
                bookingRequest.IsActive = false;
            _context.BookingRequests.Update(bookingRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<HostBookingRequestDTO>> GetBookingRequestsByHostIdAsync(string hostId)
        {
            var requests = await _context.BookingRequests
                .Where(br => _context.Listings.Any(l => l.Id == br.ListingId && l.HostId == hostId))
                .Select(br => new HostBookingRequestDTO
                {
                    BookingRequestId = br.Id,
                    GuestId = br.GuestId,
                    GuestName = _context.Users.Where(u => u.Id == br.GuestId).Select(u => u.UserName).FirstOrDefault(),
                    ListingTitle = _context.Listings.Where(l => l.Id == br.ListingId).Select(l => l.Title).FirstOrDefault(),
                    RoomTitle = _context.Rooms.Where(r => r.Id == br.RoomId).Select(r => r.Name).FirstOrDefault(),
                    BedTitle = _context.Beds.Where(b => b.Id == br.BedId).Select(b => b.Label).FirstOrDefault(),
                    ListingLocation = _context.Listings.Where(l => l.Id == br.ListingId)
                        .Select(l => l.Governorate + " - " + l.District).FirstOrDefault(),
                    FromDate = (DateTime)br.FromDate,
                    ToDate = (DateTime)br.ToDate,
                    IsApproved = br.HostApproved.HasValue ? br.HostApproved.Value.ToString() : "false"
                    //IsApproved = (bool)br.HostApproved
                })
                .ToListAsync();

            return requests;
        }


        public async Task<ChatWithHostDTO?> GetLatestBookingRequestAsync(int listingId, string guestId)
        {
            var latestRequest = await _context.BookingRequests
                .Where(br => br.ListingId == listingId && br.GuestId == guestId)
                .OrderByDescending(br => br.FromDate)
                .Select(br => new ChatWithHostDTO
                {
                    ListingId = br.ListingId,
                    ListingTitle = br.Listing.Title ?? "Unknown",
                    HostId = br.Listing.HostId,
                    HostName = br.Listing.Host.UserName ?? "Unknown",
                    GuestId = br.GuestId,
                    GuestName = br.Guest.UserName ?? "Unknown"
                })
                .FirstOrDefaultAsync();

            return latestRequest;
        }
    }
}
