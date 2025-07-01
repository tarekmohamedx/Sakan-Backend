using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class ReviewService:IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReviewAsync(CreateReviewDTO dto, string hostId)
        {
            var booking = await _reviewRepository.GetBookingWithListingAsync(dto.BookingId);

            if (booking == null)
                throw new Exception("Booking not found.");

            if (booking.Listing.HostId != hostId)
                throw new Exception("You can only review bookings from your own listings.");

            if (booking.GuestId != dto.ReviewedUserId)
                throw new Exception("Reviewed user is not the guest of this booking.");

            if (await _reviewRepository.HasHostAlreadyReviewedAsync(dto.BookingId, hostId))
                throw new Exception("You have already reviewed this guest for this booking.");

            var review = new Review
            {
                ReviewerId = hostId,
                ReviewedUserId = dto.ReviewedUserId,
                BookingId = dto.BookingId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _reviewRepository.AddReviewAsync(review);
            await _reviewRepository.SaveChangesAsync();
        }
    }
}
