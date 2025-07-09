
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {

        public bool IsDeleted { get; set; } = false;


        // Add any additional properties or methods specific to your application here
       // public string ProfilePictureUrl { get; set; }
        public virtual ICollection<Review> ReviewReviewers { get; set; } 
        public virtual ICollection<Review> ReviewReviewedUsers { get; set; } 

        public virtual ICollection<Message> MessageSenders { get; set; }
        public virtual ICollection<Message> MessageReceivers { get; set; }

        public virtual ICollection<Listing> Listings { get; set; }

        public virtual ICollection<BookingRequest> BookingRequests { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Bed> Beds { get; set; }



    }
}
