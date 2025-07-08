using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class ChatWithHostDTO
    {
        public int? ListingId { get; set; }
        public string ListingTitle { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string GuestId { get; set; }
        public string GuestName { get; set; }
    }

}
