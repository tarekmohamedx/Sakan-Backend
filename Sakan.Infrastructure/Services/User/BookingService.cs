using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces.User;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.User
{
    public class BookingService : IBookingService
    {
        private readonly sakanContext context;

        public BookingService(sakanContext context)
        {
            this.context = context;
        }

        public async Task<bool> IsAvailableAsync(CheckAvailabilityDto dto)
        {
            var query = context.Bookings.AsQueryable();

            if (dto.BedId.HasValue)
                query = query.Where(b => b.BedId == dto.BedId);
            else if (dto.RoomId.HasValue)
                query = query.Where(b => b.RoomId == dto.RoomId);
            else if (dto.ListingId.HasValue)
                query = query.Where(b => b.ListingId == dto.ListingId);
            else
                throw new Exception("At least one of ListingId, RoomId, or BedId must be provided.");

            query = query.Where(b =>
                b.FromDate < dto.ToDate &&
                b.ToDate > dto.FromDate &&
                b.IsActive == true &&
                b.PaymentStatus == "Paid" // Only count paid bookings
            );

            return !await query.AnyAsync(); 
        }
    }
}
