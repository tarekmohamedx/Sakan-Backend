using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Models
{
    public class ListingAmenities
    {
        public int listingId { get; set; }
        public Listing listing { get; set; }
        public int AmenitiesId { get; set; }
        public Amenity amenity { get; set; }
    }
}
