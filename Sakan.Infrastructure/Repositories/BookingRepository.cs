using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;

namespace Sakan.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly sakanContext _context; // استخدم اسم الـ DbContext الخاص بك

        public BookingRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<Booking> GetByIdAsync(int bookingId)
        {
            // تم التعديل هنا ليتأكد من استخدام اسم الكلاس الصحيح ApplicationUser
            // الذي تم الإشارة إليه في موديل الحجز الخاص بك عبر الخاصية Guest
            return await _context.Bookings
                .Include(b => b.Listing)
                .Include(b => b.Guest) // اسم الخاصية Guest صحيح حسب الموديل الخاص بك
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.GuestId == userId)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.ListingPhotos)
                .OrderByDescending(b => b.FromDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }
    }
}
