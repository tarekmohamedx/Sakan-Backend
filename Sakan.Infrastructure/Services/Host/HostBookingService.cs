using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.Host;
using Sakan.Application.Interfaces.Host;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.Host
{
    public class HostBookingService : IHostBookingService
    {
        private readonly sakanContext _context;

        public HostBookingService(sakanContext context)
        {
            _context = context;
        }
        public async Task<List<HostBookingViewDto>> GetHostBookingsAsync(string hostId)
        {
            return await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                .Include(b => b.Room)
                .Include(b => b.Bed)
                .Where(b => b.Listing.HostId == hostId && b.IsActive)
                .Select(b => new HostBookingViewDto
                {
                    Id = b.Id,
                    GuestName = b.Guest.UserName,
                    GuestEmail = b.Guest.Email,
                    ListingTitle = b.Listing.Title,
                    RoomName = b.Room.Name,
                    BedLabel = b.Bed.Label,
                    FromDate = b.FromDate,
                    ToDate = b.ToDate,
                    Price = b.Price,
                    PaymentStatus = b.PaymentStatus,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

    }
}
