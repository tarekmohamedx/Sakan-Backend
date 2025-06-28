using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class CreateChatRequestDTO
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int ListingId { get; set; }
    }
}
