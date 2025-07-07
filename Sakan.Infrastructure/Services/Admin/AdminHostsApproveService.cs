using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.Admin;
using Sakan.Application.Interfaces.Admin;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.Admin
{
    public class AdminHostsApproveService : IAdminHostsService
    {
        private readonly sakanContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminHostsApproveService(sakanContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<HostAdminViewDto>> GetAllHostsAsync()
        {
            return await _context.Users
                .Where(u => u.HostVerificationStatus != null)
                .Select(u => new HostAdminViewDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    HostVerificationStatus = u.HostVerificationStatus
                }).ToListAsync();
        }

        public async Task<string> HandleHostApprovalAsync(AdminHostApprovalDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return "User not found";

            if (dto.Action == "approve")
            {
                user.HostVerificationStatus = "accepted";
                await _userManager.AddToRoleAsync(user, "Host");
            }
            else if (dto.Action == "reject")
            {
                user.HostVerificationStatus = "rejected";
            }

            await _context.SaveChangesAsync();
            return $"Host {dto.Action}ed successfully.";
        }
    }
}

