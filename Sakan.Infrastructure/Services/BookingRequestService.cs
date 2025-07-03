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

        public async Task<(int requestId, string hostId)> CreateAsync(BookingRequestsDto dto)
        {
            int firstRequestId = 0;

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

                // Save the first inserted ID to return
                if (firstRequestId == 0)
                    firstRequestId = booking.Id;
            }

            // Get the host ID once
            string hostId = await _context.Listings
                .Where(l => l.Id == dto.ListingId)
                .Select(l => l.HostId)
                .FirstOrDefaultAsync();

            return (firstRequestId, hostId);
        }

    }
}
