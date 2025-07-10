using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class UserCreateReviewDto
    {
        public string? ReviewedUserId { get; set; } // HostId
        public int? ListingId { get; set; }         // Listing being reviewed
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class BookingReviewDto
    {
        public int BookingId { get; set; }
        public string ListingTitle { get; set; }
        public int? ListingId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public bool HasReview { get; set; }
        public string ReviewedUserId { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
    }

}
