using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class ListingSummaryDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal? StartingPrice { get; set; }
        public string? MainPhotoUrl { get; set; }
        public double AverageRating { get; set; }
        public string? Governorate { get; set; }
        public string? District { get; set; }
    }
}
