using Sakan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class HostDashboardService : IHostDashboardService
    {
        public HostDashboardService(IHostDashboard hostDashboardRepo)
        {
            HostDashboardRepo = hostDashboardRepo;
        }

        public IHostDashboard HostDashboardRepo { get; }

        public Task<int> GetRequestedCountAsync(string userId)
        {
            return HostDashboardRepo.GetRequestedCountAsync(userId);
        }
    }
}
