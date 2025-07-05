using Sakan.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.User
{
    public interface IReviewService
    {
      public Task AddReviewAsync(CreateReviewDTO dto, string hostId);

    }
}
