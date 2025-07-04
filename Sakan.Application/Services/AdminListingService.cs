using Sakan.Application.DTOs;
using Sakan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class AdminListingService : IAdminListingService
    {
        public AdminListingService(IListingRepository listingRepository)
        {
            ListingRepository = listingRepository;
        }

        public IListingRepository ListingRepository { get; }

        public async Task<(List<AdminListingsTableDTO> Items, int TotalCount)> GetNotApprovedListingsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var result = await ListingRepository.GetAllNotApprovedListingAsync(pageNumber, pageSize);

                var mappedItems = result.Items.Select(l => new AdminListingsTableDTO
                {
                    Id = l.Id,
                    HostId = l.HostId,
                    HostName = l.Host?.UserName ?? "Unknown",
                    Title = l.Title,
                    Description = l.Description,
                    PricePerMonth = l.PricePerMonth,
                    CreatedAt = l.CreatedAt,
                    PhotoUrl = l.ListingPhotos.FirstOrDefault()?.PhotoUrl ?? string.Empty
                }).ToList();

                return (mappedItems, result.TotalCount);
            }
            catch (Exception ex)
            {
                // Log it here
                Console.WriteLine(ex);
                throw; // or return empty with count = 0 if preferred
            }
        }

        public async Task<bool> ApproveListingAsync(int listingId)
        {
            return await ListingRepository.ApproveExistingListing(listingId);
        }

        public async Task<bool> RejectListingAsync(int listingId)
        {
            return await ListingRepository.RejectExistingListing(listingId);
        }
    }
}
