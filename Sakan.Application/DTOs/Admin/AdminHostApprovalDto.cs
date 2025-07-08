using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.Admin
{
    public class AdminHostApprovalDto
    {
        public string UserId { get; set; }
        public string Action { get; set; }
    }

    public class HostAdminViewDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HostVerificationStatus { get; set; }
    }
}
