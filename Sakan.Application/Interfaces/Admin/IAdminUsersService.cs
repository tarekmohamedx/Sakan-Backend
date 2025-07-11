using Sakan.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.Admin
{
   public interface IAdminUsersService
    {
        Task<List<HostAdminViewDto>> GetAllHostsAsync();
        Task<List<GuestAdminViewDto>> GetAllGuestsAsync();
        Task<bool> UpdateUserAsync(string userId, string userName, string email);
        Task<bool> DeleteUserAsync(string userId);
    }
}
