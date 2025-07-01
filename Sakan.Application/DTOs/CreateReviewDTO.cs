using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class CreateReviewDTO
    {
        public string ReviewedUserId { get; set; }  
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
