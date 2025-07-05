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
    }

    public class ApproveBookingRequest
    {
        public int ChatId { get; set; }
        public bool IsHost { get; set; }
    }
}
