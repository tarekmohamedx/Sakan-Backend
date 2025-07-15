using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class BookingApprovalResult
    {
        public bool GuestApproved { get; set; }
        public bool HostApproved { get; set; }
        public string Status { get; set; } // "GoToPayment", "PendingHost", etc.
        public string? ListingTitle { get; set; }
        public string? ApproverName { get; set; }   
        public string? ApproverId { get; set; }
        public string? UserIdToNotify { get; set; }
    }

    public class ApproveBookingRequest
    {
        public int? ChatId { get; set; }
        public bool IsHost { get; set; }
        public int BookingId { get; set; }              
        public string? UserId { get; set; }
    }
}
