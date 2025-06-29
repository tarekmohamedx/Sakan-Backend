using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public interface IHostDashboardService
    {
        Task<int> GetRequestedCountAsync(string userId);
    }
}
