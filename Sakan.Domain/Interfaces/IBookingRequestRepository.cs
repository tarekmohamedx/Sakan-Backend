using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Models;

namespace Sakan.Domain.Interfaces
{
    public interface IBookingRequestRepository
    {
        Task<BookingRequest> GetByIdAsync(int requestId);
        Task AddAsync(BookingRequest bookingRequest);
    }
}
