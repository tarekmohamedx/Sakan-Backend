using Sakan.Application.DTOs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.Host
{
    public interface IHostReviewsService
    {
        Task<List<HostReviewDto>> GetUserReviewsByHostAsync(string hostId);
        Task<bool> CreateReviewAsync(string reviewerId, ReviewDto dto);

        Task<List<HostReviewDto>> GetMyReviewsAsync(string hostId);
        Task<bool> CreateOrUpdateReviewAsync(string reviewerId, ReviewDto dto);
    }
}
