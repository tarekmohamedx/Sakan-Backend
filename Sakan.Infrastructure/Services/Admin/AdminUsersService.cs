using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.Admin;
using Sakan.Application.Interfaces.Admin;
using Sakan.Domain.Models;

public class AdminUsersService : IAdminUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUsersService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<HostAdminViewDto>> GetAllHostsAsync()
    {
        var hosts = await _userManager.Users
            .Where(u => u.HostVerificationStatus != null)
            .Select(u => new HostAdminViewDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                HostVerificationStatus = u.HostVerificationStatus,
                PhoneNumber=u.PhoneNumber,
                IsDeleted=u.IsDeleted
            })
            .ToListAsync();

        return hosts;
    }

    public async Task<List<GuestAdminViewDto>> GetAllGuestsAsync()
    {
        var guests = await _userManager.Users
            .Where(u => u.HostVerificationStatus == null)
            .Select(u => new GuestAdminViewDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsDeleted = u.IsDeleted

            })
            .ToListAsync();

        return guests;
    }

    public async Task<bool> UpdateUserAsync(string userId, string userName, string email, string PhoneNumber)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        user.UserName = userName;
        user.Email = email;
        user.PhoneNumber = PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        user.IsDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
