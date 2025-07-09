using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class CreateBedDTO
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }

        public List<IFormFile> BedPhotos { get; set; } = new();
    }
}
