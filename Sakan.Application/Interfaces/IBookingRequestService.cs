using Sakan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces
{
    public interface IBookingRequestService
    {
        Task<(int requestId, string hostId)> CreateAsync(BookingRequestsDto dto);
        Task<bool> UpdateBookingRequestAsync(int requestId, bool isAccepted);

        Task<IEnumerable<BookingRequestsDTO>> GetBookingRequestsByUserIdAsync(string userId);

        Task<IEnumerable<HostBookingRequestDTO>> GetBookingRequestsByHostIdAsync(string hostId);
    }

}
