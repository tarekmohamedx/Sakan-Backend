using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class BookingRequestsDTO
    {
        public int BookingRequestId { get; set; }
        public string ListingTitle { get; set; }
        public decimal? BedPrice { get; set; }
        public string ListingLocation { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
}
