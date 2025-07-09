using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Models
{
    public class UserChatSummary
    {
        public int ChatId { get; set; }
        public int ListingId { get; set; }
        public string ListingTitle { get; set; } = "Mansoura Apartment";
        public string ListingStatus { get; set; } = "Pending";
        public string HostName { get; set; } = "DefaultHost";
        public string UserName { get; set; } = "DefaultUserName";
        public LastMessageInfo? LastMessage { get; set; }
    }

    public class LastMessageInfo
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
    }

}
