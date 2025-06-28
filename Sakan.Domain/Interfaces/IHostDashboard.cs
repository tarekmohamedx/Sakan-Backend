using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Interfaces
{
    public interface IHostDashboard
    {
        Task<int> GetRequestedCountAsync(string userId);
    }
}
