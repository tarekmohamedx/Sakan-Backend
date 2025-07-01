using Sakan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces
{
    public interface IHostReviewsService
    {
        Task<List<HostReviewDto>> GetUserReviewsByHostAsync(string hostId);
        Task<bool> CreateReviewAsync(string reviewerId, ReviewDto dto);

        Task<List<HostReviewDto>> GetMyReviewsAsync(string hostId);
        Task<bool> CreateOrUpdateReviewAsync(string reviewerId, ReviewDto dto);
    }
}
