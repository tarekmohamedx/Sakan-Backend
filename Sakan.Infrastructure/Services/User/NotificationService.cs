using Microsoft.EntityFrameworkCore;
using Sakan.Application.Interfaces.User;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.User
{
    public class NotificationService : INotificationService
    {
        private readonly sakanContext _context;

        public NotificationService(sakanContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    Link = n.Link,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null) return false;

            notif.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
