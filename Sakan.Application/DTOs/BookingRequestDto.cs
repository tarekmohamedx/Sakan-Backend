using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class BookingRequestDto
    {
        public string GuestId { get; set; }
        public int? ListingId { get; set; }
        public int? RoomId { get; set; }
        public int? BedId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

}
