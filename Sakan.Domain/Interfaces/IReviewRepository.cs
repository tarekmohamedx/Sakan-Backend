using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<Booking> GetBookingWithListingAsync(int bookingId);
        Task<bool> HasHostAlreadyReviewedAsync(int bookingId, string reviewerId);
        Task AddReviewAsync(Review review);
        Task SaveChangesAsync();
    }
}
