using Sakan.Application.DTOs;
using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces
{
    public interface IHostListingService
    {
        Task<bool> UpdateListingWithPhotosAsync(int id, string hostId, ListingEditDto updated);
        Task<bool> DeleteListingAsync(int id, string hostId);
        Task<List<HostListingDto>> GetHostListingsAsync(string hostId);
        Task<ListingEditDto> GetListingByIdAsync(int id, string hostId);
    }
}
