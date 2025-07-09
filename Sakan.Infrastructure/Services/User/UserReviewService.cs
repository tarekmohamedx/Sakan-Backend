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
    public class UserReviewService : IUserReviewService
    {
        private readonly sakanContext _context;

        public UserReviewService(sakanContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateOrUpdateReviewAsync(string reviewerId, UserCreateReviewDto dto)
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

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReviewDetailsDto>> GetReviewsByListingIdAsync(int listingId)
        {
            return await _context.Reviews
                .Where(r => r.ListingId == listingId && r.IsActive)
                .Include(r => r.Reviewer)
                .Include(r => r.Listing)
                .Select(r => new ReviewDetailsDto
                {
                    ReviewerName = r.Reviewer.UserName,
                    ListingId = r.ListingId ?? 0,
                    ListingTitle = r.Listing.Title,
                    Rating = r.Rating ?? 0,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt ?? DateTime.Now
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<ReviewDetailsDto>> GetUserReviewsAsync(string userId)
        {
            return await _context.Reviews
                .Where(r => r.ReviewerId == userId && r.IsActive)
                .Select(r => new ReviewDetailsDto
                {
                    ReviewerName = r.Reviewer.UserName,
                    ListingTitle = r.Listing.Title,
                    Rating = r.Rating ?? 0,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt ?? DateTime.Now
                }).ToListAsync();
        }

        public async Task<IEnumerable<BookingReviewDto>> GetUserBookingsWithReviewStatusAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.GuestId == userId)
                .Select(b => new BookingReviewDto
                {
                    BookingId = b.Id,
                    ListingId = b.ListingId,
                    ListingTitle = b.Listing.Title,
                    FromDate = DateOnly.FromDateTime(b.FromDate),
                    ToDate = DateOnly.FromDateTime(b.ToDate),

                    // ✅ Fix here: Only reviews by the current user
                    HasReview = b.Reviews.Any(r => r.ReviewerId == userId && r.IsActive),
                    ReviewedUserId = b.Listing.HostId,
                    Rating = b.Reviews
                        .Where(r => r.ReviewerId == userId && r.IsActive)
                        .Select(r => r.Rating)
                        .FirstOrDefault(),

                    Comment = b.Reviews
                        .Where(r => r.ReviewerId == userId && r.IsActive)
                        .Select(r => r.Comment)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

    }
}
