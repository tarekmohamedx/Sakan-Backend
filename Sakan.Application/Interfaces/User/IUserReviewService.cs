using Sakan.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.User
{
    public interface IUserReviewService
    {
        //Task<bool> CreateReviewAsync(UserCreateReviewDto dto);
        Task<bool> CreateOrUpdateReviewAsync(string reviewerId, UserCreateReviewDto dto);
        Task<IEnumerable<ReviewDetailsDto>> GetReviewsByListingIdAsync(int listingId);
        Task<IEnumerable<ReviewDetailsDto>> GetUserReviewsAsync(string userId);
        Task<IEnumerable<BookingReviewDto>> GetUserBookingsWithReviewStatusAsync(string userId);
    }
}
