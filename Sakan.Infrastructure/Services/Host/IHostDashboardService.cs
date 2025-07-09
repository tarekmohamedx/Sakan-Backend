using Sakan.Application.DTOs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.Host
{
    internal interface IHostDashboardService
    {
        Task<HostDashboardDTO> GetDashboardAsync(string hostId);

    }
}
