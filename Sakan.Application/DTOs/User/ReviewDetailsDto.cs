using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class ReviewDetailsDto
    {
        public string? ReviewerName { get; set; }
        public int ListingId { get; set; }
        public string ListingTitle { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
