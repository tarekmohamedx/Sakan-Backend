using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.User
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
