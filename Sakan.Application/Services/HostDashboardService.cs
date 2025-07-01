using Sakan.Application.DTOs;
using Sakan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class HostDashboardService : IHostDashboardService
    {
        public HostDashboardService(IHostDashboard hostDashboardRepo)
        {
            HostDashboardRepo = hostDashboardRepo;
        }

        public IHostDashboard HostDashboardRepo { get; }

        public async Task<HostDashboardDTO> GetDashboardAsync(string hostId)
        {
            var activeListingsCount = await HostDashboardRepo.GetActiveListingCount(hostId);
            var approvedListingsCount = await HostDashboardRepo.GetApprovedListingCount(hostId);
            var totalBookings = await HostDashboardRepo.GetTotalBookingsAsync(hostId);
            var monthlyRevenue = await HostDashboardRepo.GetThisMonthRevenue(hostId);
            var occupancyRate = await HostDashboardRepo.GetOccupancyRate(hostId);
            var avgRating = await HostDashboardRepo.GetAverageRatingForHost(hostId);
            var latestReview = await HostDashboardRepo.GetLatestReviewForHostAsync(hostId);
            var todaysRequests = await HostDashboardRepo.GetTodaysRequestsAsync(hostId);
            var totalRequestsCount = await HostDashboardRepo.GetRequestedCountAsync(hostId);

            var recentRequests = todaysRequests
                    .OrderByDescending(br => br.Id)
                    .Take(5)
                    .Select(br => new BookingRequestDto
                    {
                        GuestName = br.Guest?.UserName ?? br.Guest?.UserName ?? "Unknown",
                        GuestId = br.Guest?.Id ?? "Unknown",
                        ListingTitle = br.Listing?.Title ?? "N/A",
                        ListingId = br.ListingId ?? 0,
                        RoomName = br.Room?.Name,
                        RoomId = br.Room?.Id,
                        BedId = br.BedId,
                        FromDate = br.FromDate!.Value,
                        ToDate = br.ToDate!.Value
                    })
                    .ToList();

            //ReviewDetailsDto LatestReviewDTO = new ReviewDetailsDto
            //{
            //    ReviewerName = latestReview.Reviewer?.UserName ?? "Unknown",
            //    ListingId = latestReview.Booking.Listing.Id,
            //    ListingTitle = latestReview.Booking.Listing.Title,
            //    Rating = (double)latestReview.Rating,
            //    Comment = latestReview.Comment,
            //    CreatedAt = (DateTime)latestReview.CreatedAt
            //};

            ReviewDetailsDto LatestReviewDTO = latestReview != null ? new ReviewDetailsDto
            {
                ReviewerName = latestReview.Reviewer?.UserName ?? "Unknown",
                ListingId = latestReview.Booking?.Listing?.Id ?? 0,
                ListingTitle = latestReview.Booking?.Listing?.Title ?? "N/A",
                Rating = (double?)latestReview.Rating ?? 0,
                Comment = latestReview.Comment ?? "No comment",
                CreatedAt = latestReview.CreatedAt ?? DateTime.MinValue
            } : null;

            return new HostDashboardDTO
            {
                ActiveListingsCount = activeListingsCount,
                ApprovedListingsCount = approvedListingsCount,
                TotalBookings = totalBookings,
                MonthlyRevenue = monthlyRevenue,
                OccupancyRate = occupancyRate,
                AverageRating = avgRating,
                LatestReview = LatestReviewDTO,
                TodaysRequestsCount = todaysRequests.Count,
                RecentRequests = recentRequests
            }; ;
        }

        public Task<int> GetRequestedCountAsync(string userId)
        {
            return HostDashboardRepo.GetRequestedCountAsync(userId);
        }
    }
}
