using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.Host
{
    public class HostListingDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal PricePerMonth { get; set; }
        public int MaxGuests { get; set; }
        public string PreviewImage { get; set; }
        public bool IsActive { get; set; }

    }

    public class ListingEditDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? PricePerMonth { get; set; }
        public int? MaxGuests { get; set; }
        public string Governorate { get; set; }
        public string District { get; set; }
        public bool? IsBookableAsWhole { get; set; }
        public bool IsActive { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }

    public class AdminHostListingDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal PricePerMonth { get; set; }
        public int MaxGuests { get; set; }
        public bool IsActive { get; set; }
        public string PreviewImage { get; set; }
        public bool IsApproved { get; set; }
        public string HostName { get; set; } // ✅ Add this
    }


}
