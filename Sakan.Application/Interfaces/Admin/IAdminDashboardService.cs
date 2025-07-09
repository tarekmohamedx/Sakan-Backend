using Sakan.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.Admin
{
    public interface IAdminDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<IEnumerable<ActivityLogDto>> GetRecentActivityAsync();

    }

}
