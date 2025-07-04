using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Common;
using Sakan.Domain.Models;

namespace Sakan.Domain.Interfaces
{
    public interface IListingRepository
    {
        // --- تعديل هنا: نرجع مباشرة لكائن الـ Listing ---
        Task<(List<Listing> Items, int TotalCount)> GetFilteredListingsAsync(ListingFilterParameters filterParams);
        Task<(List<Listing> Items, int TotalCount)> GetAllListingsAsync(int pageNumber, int pageSize);

        // --- تعديل هنا أيضاً لواجهات الصفحة الرئيسية ---
        Task<List<Listing>> GetHighestRatedListingsAsync(int count);
        Task<List<Listing>> GetNewestListingsAsync(int count);
        Task<List<Listing>> GetMostAffordableListingsAsync(int count);
        Task<(List<Listing> Items, int TotalCount)> GetAllNotApprovedListingAsync(int pageNumber, int pageSize);

        Task<bool> ApproveExistingListing(int listingId);
        Task<bool> RejectExistingListing(int listingId);
    }
}
