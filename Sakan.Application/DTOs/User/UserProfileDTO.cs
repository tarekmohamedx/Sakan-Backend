using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class UserProfileDTO
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string ReservedType { get; set; }
        public string ReservedName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Governorate { get; set; }
        public string District { get; set; }
        public bool? Hostapproved { get; set; }
        public bool? Guestapproved { get; set; }

    }
}
