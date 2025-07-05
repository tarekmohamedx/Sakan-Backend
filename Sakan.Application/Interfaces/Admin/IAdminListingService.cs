using Sakan.Application.DTOs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.Admin
{
    public interface IAdminListingService
    {
        Task<ListingEditDto> GetListingByIdAsync(int id);
        Task<object> GetAlltListingsAsync(int page, int pageSize, string? search);
        Task<bool> UpdateListingWithPhotosAsync(int id, ListingEditDto updated);
        Task<bool> DeleteListingAsync(int id);
        Task<bool> SetListingApprovalStatusAsync(int listingId, bool isApproved);
    }
}
