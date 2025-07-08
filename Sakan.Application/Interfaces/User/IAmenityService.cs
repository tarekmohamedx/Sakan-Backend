using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.DTOs.User;

namespace Sakan.Application.Interfaces.User
{
    public interface IAmenityService
    {
        Task<List<AmenityDto>> GetAllAsync();
    }
}
