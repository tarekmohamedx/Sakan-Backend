using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.DTOs.Host;
using Sakan.Application.DTOs.User;

namespace Sakan.Application.Interfaces.User
{
    public interface IListingDetailsService
    {
        Task<ListingDetailsDto> GetListingDetails(int id);

        Task<List<BookedMonthDto>> GetBookedMonthsAsync(int listingId);

        Task<List<ReviewsDto>> GetReviewsForListingAsync(int listingId);

        //Task<List<AmenityDto>> GetAmenitiesForListingAsync(int listingId);


    }
}
