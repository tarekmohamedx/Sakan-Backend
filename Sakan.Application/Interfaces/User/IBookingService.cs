using Sakan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.User
{
    public interface IBookingService
    {
        Task<bool> IsAvailableAsync(CheckAvailabilityDto dto);
    }
}
