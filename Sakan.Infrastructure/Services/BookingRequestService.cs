using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services
{
    // BookingRequestService.cs
    public class BookingRequestService : IBookingRequestService
    {
        private readonly sakanContext _context;

        public BookingRequestService(sakanContext context)
        {
            _context = context;
        }

        public async Task<(int requestId, string hostId)> CreateAsync(BookingRequestDto dto)
        {

            var booking = new BookingRequest
            {
                GuestId = dto.GuestId,
                ListingId = dto.ListingId,
                RoomId = dto.RoomId,
                BedId = dto.BedId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                HostApproved = false,
                GuestApproved = false
            };

            _context.BookingRequests.Add(booking);
            await _context.SaveChangesAsync();

            // Try to get the host ID explicitly
            string hostId = await _context.Listings
                .Where(l => l.Id == dto.ListingId)
                .Select(l => l.HostId)
                .FirstOrDefaultAsync();

            return (booking.Id, hostId);
        }
    }

}
