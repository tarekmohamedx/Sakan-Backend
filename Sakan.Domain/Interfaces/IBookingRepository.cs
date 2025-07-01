using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Models;

namespace Sakan.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> GetByIdAsync(int bookingId);
        // ... other methods
        Task<List<Booking>> GetUserBookingsAsync(string userId);

        Task AddAsync(Booking booking);
    }
}
