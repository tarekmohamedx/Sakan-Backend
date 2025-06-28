using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.DTOs;

namespace Sakan.Application.Services_Interfaces
{
    public interface IAmenityService
    {
        Task<List<AmenityDto>> GetAllAsync();
    }
}
