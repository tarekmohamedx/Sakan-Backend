using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{

    public class RoomDto
    {
        public int Id { get; set; }
        public int? ListingId { get; set; }
        public string Name { get; set; }
        public decimal? PricePerNight { get; set; }
        public bool? IsBookableAsWhole { get; set; }
        public List<string> Photos { get; set; }
        public List<BedDto> Beds { get; set; }
        public ListingLocationDto Listing { get; set; }
    }



    public class ListingLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class BedDto
    {
        public int? Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public List<string> BedPhotos { get; set; } = new();
    }

    // for host page 
    public class RoomDetailsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal? PricePerNight { get; set; }
        public int? MaxGuests { get; set; }
        public bool? IsBookableAsWhole { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? ListingTitle { get; set; }
        public int? ListingId { get; set; }

        public List<BedDto> Beds { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new();
    }

    public class RoomUpdateDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal? PricePerNight { get; set; }
        public int? MaxGuests { get; set; }
        public bool? IsBookableAsWhole { get; set; }
        public bool IsActive { get; set; }
        public List<string> RoomPhotoUrls { get; set; } = new();
        public List<BedDto> Beds { get; set; } = new();
    }


}
