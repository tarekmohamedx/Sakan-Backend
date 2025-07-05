using Sakan.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public interface IAdminApproveListingService
    {
        Task<(List<AdminListingsTableDTO> Items, int TotalCount)> GetNotApprovedListingsAsync(int pageNumber, int pageSize);
        Task<bool> ApproveListingAsync(int listingId);
        Task<bool> RejectListingAsync(int listingId);
    }
}
