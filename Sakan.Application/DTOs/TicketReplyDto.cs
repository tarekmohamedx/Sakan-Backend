using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs
{
    public class TicketReplyDto
    {
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
