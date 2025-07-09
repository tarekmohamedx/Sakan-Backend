using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class CreateListingDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal? PricePerMonth { get; set; }
        public int? MaxGuests { get; set; }
        public string Governorate { get; set; }
        public string District { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool? IsBookableAsWhole { get; set; }

        public List<CreateRoomDTO> Rooms { get; set; } = new();
        public List<int> AmenityIds { get; set; } 

        public List<IFormFile> ListingPhotos { get; set; } = new();
    }
}
