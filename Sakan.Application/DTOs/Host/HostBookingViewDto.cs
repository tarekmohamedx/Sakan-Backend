using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.Host
{
    public class HostBookingViewDto
    {
        public int Id { get; set; }
        public string GuestName { get; set; }
        public string GuestEmail { get; set; }
        public string ListingTitle { get; set; }
        public string? RoomName { get; set; }
        public string? BedLabel { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal? Price { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

}
