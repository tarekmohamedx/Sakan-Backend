using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class HostBookingRequestDTO
    {

        public int BookingRequestId { get; set; }
        public string GuestId { get; set; }
        public string GuestName { get; set; }
        public string ListingTitle { get; set; }
        public string RoomTitle { get; set; }
        public string BedTitle { get; set; }
        public string ListingLocation { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string IsApproved { get; set; }
    }
}
