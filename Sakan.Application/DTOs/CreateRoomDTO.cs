using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class CreateRoomDTO
    {
        [Required]
        public string Name { get; set; }

        public string Type { get; set; }
        public decimal? PricePerNight { get; set; }
        public int? MaxGuests { get; set; }
        public bool? IsBookableAsWhole { get; set; }

        public List<CreateBedDTO> Beds { get; set; } = new();
        public List<IFormFile> RoomPhotos { get; set; } = new();
    }
}
