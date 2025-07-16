using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class BookingRQSDTO
    {
        public int Id { get; set; }
        public string GuestId { get; set; }
        public int? ListingId { get; set; }

        public int? RoomId { get; set; }
        public int? BedId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
    }
}
