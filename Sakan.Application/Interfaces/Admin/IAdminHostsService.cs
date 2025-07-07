using Sakan.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.Admin
{
    public interface IAdminHostsService
    {
        Task<List<HostAdminViewDto>> GetAllHostsAsync();
        Task<string> HandleHostApprovalAsync(AdminHostApprovalDto dto);
    }
}
