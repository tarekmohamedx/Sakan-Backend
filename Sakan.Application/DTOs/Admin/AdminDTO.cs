using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.Admin
{
    class AdminDTO
    {
    }

    public class DashboardSummaryDto
    {
        public int TotalHosts { get; set; }
        public int TotalGuests { get; set; }
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int PendingListings { get; set; }
        public int RejectedListings { get; set; }
        public int ApprovedListings { get; set; }
        public int PendingApprovals { get; set; }
        public int OpenComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int UnreadMessages { get; set; }
    }

    public class ActivityLogDto
    {
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
