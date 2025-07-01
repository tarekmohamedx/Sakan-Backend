using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Interfaces
{
    public interface IHostDashboard
    {
        Task<int> GetRequestedCountAsync(string userId);
        Task<int> GetActiveListingCount(string userId);
        Task<int> GetApprovedListingCount(string userId);
        Task<double> GetOccupancyRate(string userId);
        Task<decimal> GetThisMonthRevenue(string userId);
        Task<List<BookingRequest>> GetTodaysRequestsAsync(string hostId);
        Task<double> GetAverageRatingForHost(string hostId);
        Task<Review> GetLatestReviewForHostAsync(string hostId);
        Task<int> GetTotalBookingsAsync(string hostId);

    }
}
