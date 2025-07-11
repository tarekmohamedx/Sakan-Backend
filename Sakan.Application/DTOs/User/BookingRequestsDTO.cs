using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class BookingRequestsDTO
    {
        public string GuestId { get; set; }
        public string HostId { get; set; }
        public int BookingRequestId { get; set; }
        public string ListingTitle { get; set; }
        public decimal? BedPrice { get; set; }
        public string ListingLocation { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } // "Pending", "Accepted", "Rejected"

    }
}
