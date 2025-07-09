using Sakan.Application.DTOs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.Host
{
    public interface IHostBookingService
    {
        Task<List<HostBookingViewDto>> GetHostBookingsAsync(string hostId);
    }
}
