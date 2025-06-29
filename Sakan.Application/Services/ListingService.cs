using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sakan.Application.Common;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Common;
using Sakan.Domain.Interfaces;

namespace Sakan.Application.Services
{
    public class ListingService : IListingService
    {
        private readonly IListingRepository _listingRepository;
        private readonly IMapper _mapper; // حقن (Inject) الـ IMapper interface

        public ListingService(IListingRepository listingRepository, IMapper mapper)
        {
            _listingRepository = listingRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ListingSummaryDto>> GetFilteredListingsAsync(ListingFilterParameters filterParams)
        {
            var (listings, totalCount) = await _listingRepository.GetFilteredListingsAsync(filterParams);

            // استخدام AutoMapper للتحويل
            var dtos = _mapper.Map<List<ListingSummaryDto>>(listings);

            return new PagedResult<ListingSummaryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            };
        }

        public async Task<PagedResult<ListingSummaryDto>> GetAllListingsAsync(int pageNumber, int pageSize)
        {
            var (listings, totalCount) = await _listingRepository.GetAllListingsAsync(pageNumber, pageSize);

            // استخدام AutoMapper للتحويل
            var dtos = _mapper.Map<List<ListingSummaryDto>>(listings);

            return new PagedResult<ListingSummaryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<ListingSummaryDto>> GetHighestRatedListingsAsync(int count)
        {
            var listings = await _listingRepository.GetHighestRatedListingsAsync(count);
            // استخدام AutoMapper مباشرة
            return _mapper.Map<List<ListingSummaryDto>>(listings);
        }

        public async Task<List<ListingSummaryDto>> GetNewestListingsAsync(int count)
        {
            var listings = await _listingRepository.GetNewestListingsAsync(count);
            return _mapper.Map<List<ListingSummaryDto>>(listings);
        }

        public async Task<List<ListingSummaryDto>> GetMostAffordableListingsAsync(int count)
        {
            var listings = await _listingRepository.GetMostAffordableListingsAsync(count);
            return _mapper.Map<List<ListingSummaryDto>>(listings);
        }
    }
}
