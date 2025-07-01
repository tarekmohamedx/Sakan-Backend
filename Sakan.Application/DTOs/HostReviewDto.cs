using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class HostReviewDto
    {
        public int BookingId { get; set; }
        public string GuestId { get; set; }
        public string GuestName { get; set; }
        public string? Comment { get; set; }
        public int? Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ReviewDto
    {
        public int BookingId { get; set; }
        public string ReviewedUserId { get; set; }  // guest ID
        public int Rating { get; set; }
        public string Comment { get; set; }
    }


}
