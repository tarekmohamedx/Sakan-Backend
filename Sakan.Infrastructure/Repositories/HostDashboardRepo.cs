using Sakan.Domain.Interfaces;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Repositories
{
    public class HostDashboardRepo : IHostDashboard
    {
        public HostDashboardRepo(sakanContext _context)
        {
            Context = _context;
        }

        public sakanContext Context { get; }

        public async Task<int> GetRequestedCountAsync(string userId)
        {
            return await Task.FromResult(Context.BookingRequests
                .Count(br => br.Listing.HostId == userId && br.IsActive));
        }
    }
}
