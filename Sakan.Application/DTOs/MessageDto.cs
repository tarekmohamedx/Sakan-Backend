using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class MessageDto
    {
        public int? MessageID { get; set; }

        public string? SenderID { get; set; }

        public string ReceiverID { get; set; }

        public string Content { get; set; }

        public DateTime? Timestamp { get; set; }

        public int? ChatId { get; set; }
    }
}
