using Microsoft.EntityFrameworkCore;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly sakanContext _context;

        public ReviewRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<Booking> GetBookingWithListingAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Listing)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<bool> HasHostAlreadyReviewedAsync(int bookingId, string reviewerId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.BookingId == bookingId && r.ReviewerId == reviewerId);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
