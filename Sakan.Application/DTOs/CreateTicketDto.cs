using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class CreateTicketDto
    {
        // للزوار
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }

        // للكل
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int? BookingId { get; set; }
    }
}
