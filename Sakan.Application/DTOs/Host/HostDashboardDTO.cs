using Sakan.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.Host
{
    public class HostDashboardDTO
    {
        public int ActiveListingsCount { get; set; }
        public int ApprovedListingsCount { get; set; }
        public int TotalBookings { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public double OccupancyRate { get; set; }
        public double AverageRating { get; set; }
        public ReviewDetailsDto? LatestReview { get; set; }
        public int TodaysRequestsCount { get; set; }
        public List<BookingRequestDto> RecentRequests { get; set; } = new();


    }
}
