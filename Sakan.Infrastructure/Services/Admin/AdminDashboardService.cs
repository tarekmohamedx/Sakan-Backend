using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces.Admin;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly sakanContext _context;

        public AdminDashboardService(sakanContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var hostCount = await (
                from user in _context.Users
                join ur in _context.UserRoles on user.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Host"
                select user.Id
            ).CountAsync();

            var guestCount = await (
                from user in _context.Users
                join ur in _context.UserRoles on user.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Customer"
                select user.Id
            ).CountAsync();

            var listings = _context.Listings;
            var messages = _context.Messages;

            return new DashboardSummaryDto
            {
                TotalHosts = hostCount,
                TotalGuests = guestCount,
                TotalListings = await listings.CountAsync(),
                ActiveListings = await listings.CountAsync(l => l.IsActive),
                ApprovedListings = await listings.CountAsync(l => l.IsApproved == true),
                PendingListings = await listings.CountAsync(l => l.IsApproved == false),
                RejectedListings = 0,
                PendingApprovals = await listings.CountAsync(l => l.IsApproved == false),
                OpenComplaints = await _context.SupportTickets.CountAsync(c => c.Status != "Resolved"),
                ResolvedComplaints = await _context.SupportTickets.CountAsync(c => c.Status == "Resolved")
                //UnreadMessages = await messages.CountAsync(m => !m.IsRead)
            };
        }

        public async Task<IEnumerable<ActivityLogDto>> GetRecentActivityAsync()
        {
            var logs = new List<ActivityLogDto>();

            // User Signups - without CreatedAt
            var recentUsers = await (
                from user in _context.Users
                join ur in _context.UserRoles on user.Id equals ur.UserId
                join role in _context.Roles on ur.RoleId equals role.Id
                orderby user.Id descending
                select new ActivityLogDto
                {
                    ActivityType = $"New {role.Name} Signup",
                    Description = $"{user.UserName} joined as {role.Name}"
                }).Take(10).ToListAsync();

            logs.AddRange(recentUsers);

            // Recent Listings (assumes Listing.CreatedAt exists)
            var recentListings = await _context.Listings
                .OrderByDescending(l => l.CreatedAt)
                .Take(10)
                .Select(l => new ActivityLogDto
                {
                    ActivityType = "Listing Submitted",
                    Description = $"Listing '{l.Title}' submitted by {l.Host}",
                    Timestamp = l.CreatedAt.GetValueOrDefault()
                }).ToListAsync();

            logs.AddRange(recentListings);

            // Complaints (SupportTickets)
            var recentComplaints = await _context.SupportTickets
                .OrderByDescending(c => c.CreatedAt)
                .Take(10)
                .Select(c => new ActivityLogDto
                {
                    ActivityType = "Complaint Filed",
                    Description = $"Complaint: {c.Subject}",
                    Timestamp = c.CreatedAt
                }).ToListAsync();

            logs.AddRange(recentComplaints);

            return logs.OrderByDescending(x => x.Timestamp).Take(20);
        }

        public async Task<object> GetAlltListingsAsync(int page, int pageSize, string? search)
        {
            var query = _context.Listings
                .Where(l => !l.IsDeleted)
                .Select(l => new HostListingDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Location = l.Governorate + ", " + l.District,
                    PricePerMonth = l.PricePerMonth ?? 0,
                    MaxGuests = l.MaxGuests ?? 0,
                    IsActive = l.IsActive,
                    PreviewImage = l.ListingPhotos.FirstOrDefault().PhotoUrl
                });
            if (!string.IsNullOrEmpty(search))
                query = query.Where(l => l.Title.Contains(search));

            var total = await query.CountAsync();
            var listings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new
            {
                totalCount = total,
                page,
                pageSize,
                listings
            };
        }

    }
}
