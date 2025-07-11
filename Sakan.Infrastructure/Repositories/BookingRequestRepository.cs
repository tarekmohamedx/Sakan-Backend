using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;

namespace Sakan.Infrastructure.Repositories
{
    public class BookingRequestRepository : IBookingRequestRepository
    {
        private readonly sakanContext _context;

        public BookingRequestRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<BookingRequest> GetByIdAsync(int requestId)
        {
            // يجب أن نجلب كل البيانات المرتبطة لنستخدمها في منطق التسعير
            return await _context.BookingRequests
                .Include(br => br.Listing)
                .Include(br => br.Room)
                .Include(br => br.Bed)
                .FirstOrDefaultAsync(br => br.Id == requestId);
        }

        public async Task AddAsync(BookingRequest bookingRequest)
        {
            await _context.BookingRequests.AddAsync(bookingRequest);
        }
    }
}
