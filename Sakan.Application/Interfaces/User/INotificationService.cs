using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.User
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsAsync(string userId);
        Task CreateNotificationAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(int id);
    }
}
