using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.Common;
using Sakan.Domain.Common;
using Sakan.Application.DTOs.User;
using Microsoft.AspNetCore.Http;
namespace Sakan.Application.Interfaces.User
{
    public interface IListingService
    {
        Task CreateListingAsync(CreateListingDTO dto, string hostId);

        Task<CreateListingDTO> ParseCreateListingFormAsync(IFormCollection form);


        // --- خدمات البحث والفلترة ---
        Task<PagedResult<ListingSummaryDto>> GetFilteredListingsAsync(ListingFilterParameters filterParams);
        Task<PagedResult<ListingSummaryDto>> GetAllListingsAsync(int pageNumber, int pageSize);


        // --- خدمات الصفحة الرئيسية ---
        Task<List<ListingSummaryDto>> GetHighestRatedListingsAsync(int count);
        Task<List<ListingSummaryDto>> GetNewestListingsAsync(int count);
        Task<List<ListingSummaryDto>> GetMostAffordableListingsAsync(int count);
    }
}
