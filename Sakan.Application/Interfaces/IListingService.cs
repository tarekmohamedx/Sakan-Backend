
﻿using Sakan.Application.DTOs;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.Common;
using Sakan.Domain.Common;
namespace Sakan.Application.Interfaces
{
    public interface IListingService
    {
        Task CreateListingAsync(CreateListingDTO dto, string hostId);


        // --- خدمات البحث والفلترة ---
        Task<PagedResult<ListingSummaryDto>> GetFilteredListingsAsync(ListingFilterParameters filterParams);
        Task<PagedResult<ListingSummaryDto>> GetAllListingsAsync(int pageNumber, int pageSize);

        // --- خدمات الصفحة الرئيسية ---
        Task<List<ListingSummaryDto>> GetHighestRatedListingsAsync(int count);
        Task<List<ListingSummaryDto>> GetNewestListingsAsync(int count);
        Task<List<ListingSummaryDto>> GetMostAffordableListingsAsync(int count);
    }
}
