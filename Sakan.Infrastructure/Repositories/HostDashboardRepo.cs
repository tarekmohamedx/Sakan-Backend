using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
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
    public class HostDashboardRepo : IHostDashboard
    {
        public HostDashboardRepo(sakanContext _context)
        {
            Context = _context;
        }

        public sakanContext Context { get; }

        public async Task<int> GetActiveListingCount(string userId)
        {
            return await Context.Listings.CountAsync(l => l.HostId == userId && l.IsActive);
        }

        public async Task<int> GetApprovedListingCount(string userId)
        {
            return await Context.Listings.CountAsync(l => l.HostId == userId);
        }

        public async Task<double> GetAverageRatingForHost(string hostId)
        {
            var average = await Context.Reviews
            .Where(r => r.ReviewedUserId == hostId)
                .AverageAsync(r => (double?)r.Rating);
    
            return Math.Round(average ?? 0, 2);
        }

        public async Task<double> GetOccupancyRate(string userId)
        {
            var totalListing = await Context.Listings
                .CountAsync(l => l.HostId == userId && l.IsActive);

            if (totalListing == 0)
                return 0;

            var totalListingOccupied = await Context.Bookings
                .CountAsync(b => b.Listing.HostId == userId && b.Listing.IsActive);

            double occupancyRate = (double)totalListingOccupied / totalListing * 100;

            return Math.Round(occupancyRate, 2);
        }

        public async Task<int> GetRequestedCountAsync(string userId)
        {
            return await Task.FromResult(Context.BookingRequests
                .Count(br => br.Listing.HostId == userId && br.IsActive));
        }

        public async Task<decimal> GetThisMonthRevenue(string userId)
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1); 
            var revenue = await Context.Bookings
                .Where(b =>
                    b.Listing.HostId == userId &&
                    b.Listing.IsActive &&
                    b.CreatedAt >= firstDayOfMonth &&
                    b.CreatedAt < firstDayOfNextMonth
                )
                .SumAsync(b => (decimal?)b.Price ?? 0);

                    return revenue;
                }

        public async Task<List<BookingRequest>> GetTodaysRequestsAsync(string hostId)
        {
            var today = DateTime.UtcNow.Date;

            return await Context.BookingRequests
                .Where(br =>
                    br.IsActive &&
                    br.FromDate.HasValue &&
                    br.FromDate.Value.Date == today &&
                    br.Listing != null &&
                    br.Listing.HostId == hostId)
                .Include(br => br.Guest)
                .Include(br => br.Listing)
                .Include(br => br.Room)
                .Include(br => br.Bed)
                .ToListAsync();
        }
        public async Task<Review> GetLatestReviewForHostAsync(string hostId)
        {
            var review = await Context.Reviews
        .Where(r => r.ReviewedUserId == hostId)
        .OrderByDescending(r => r.CreatedAt)
        .Include(r => r.Reviewer)
        .Include(r => r.Booking)
            .ThenInclude(b => b.Listing)
        .FirstOrDefaultAsync();

            if (review == null || review.Booking?.Listing == null)
                return null;

            return review;
        }

        public async Task<int> GetTotalBookingsAsync(string hostId)
        {
            return await Context.Bookings
                .CountAsync(b => b.Listing != null && b.Listing.HostId == hostId);
        }
    }
}
