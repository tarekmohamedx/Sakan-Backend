using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.Host;
using Sakan.Application.Interfaces.Host;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.Host
{
    public class HostReviewsService : IHostReviewsService
    {
        private readonly sakanContext _context;

        public HostReviewsService(sakanContext context)
        {
            _context = context;
        }
        public async Task<List<HostReviewDto>> GetUserReviewsByHostAsync(string hostId)
        {
            return await _context.Reviews
                .Include(r => r.Booking)
                .Include(r => r.Reviewer)
                .Where(r => r.ReviewedUserId == hostId && r.IsActive)
                .Select(r => new HostReviewDto
                {
                    BookingId = r.BookingId,
                    GuestId = r.ReviewerId!,
                    GuestName = r.Reviewer.UserName!,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> CreateReviewAsync(string reviewerId, ReviewDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId);

            if (booking == null)
                return false;

            var guestId = booking.GuestId;

            var existing = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId && r.ReviewerId == reviewerId);

            if (existing != null)
            {
                existing.Comment = dto.Comment;
                existing.Rating = dto.Rating;
                existing.CreatedAt = DateTime.UtcNow;
                existing.IsActive = true;
            }
            else
            {
                var review = new Review
                {
                    BookingId = dto.BookingId,
                    ReviewerId = reviewerId,       // the host
                    ReviewedUserId = guestId,      // the guest
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Reviews.Add(review);
            }
            var notification = new Notification
            {
                UserId = dto.ReviewedUserId,
                Message = "You received a new review from your host.",
                Link = $"/",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HostReviewDto>> GetMyReviewsAsync(string hostId)
        {
            return await _context.Reviews
                .Include(r => r.ReviewedUser)
                .Where(r => r.ReviewerId == hostId && r.IsActive)
                .Select(r => new HostReviewDto
                {
                    BookingId = r.BookingId,
                    GuestId = r.ReviewedUserId,
                    GuestName = r.ReviewedUser.UserName,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> CreateOrUpdateReviewAsync(string reviewerId, ReviewDto dto)
        {
            var existing = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId && r.ReviewerId == reviewerId);

            if (existing != null)
            {
                existing.Comment = dto.Comment;
                existing.Rating = dto.Rating;
                existing.CreatedAt = DateTime.UtcNow;
                existing.IsActive = true;
            }
            else
            {
                var review = new Review
                {
                    BookingId = dto.BookingId,
                    ReviewerId = reviewerId,
                    ReviewedUserId = dto.ReviewedUserId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Reviews.Add(review);
            }

            var message = existing != null
                    ? "Your review has been updated by the host."
                    : "You received a new review from your host.";

            // 🔔 Always notify the reviewed user
            var notification = new Notification
            {
                UserId = dto.ReviewedUserId,
                Message = message,
                Link = $"/",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
