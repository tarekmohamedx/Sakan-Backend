using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Models;

namespace Sakan.Domain.Interfaces
{
    public interface ISupportTicketRepository
    {
        // جلب كل الشكاوى (خاص بالمسؤولين)
        Task<List<SupportTicket>> GetAllAsync();

        // جلب شكوى معينة مع كل الردود عليها
        Task<SupportTicket> GetByIdWithRepliesAsync(int ticketId);

        // جلب كل الشكاوى الخاصة بمستخدم معين
        Task<List<SupportTicket>> GetUserTicketsAsync(string userId);

        // إضافة شكوى جديدة
        Task AddTicketAsync(SupportTicket ticket);

        // إضافة رد جديد على شكوى
        Task AddReplyAsync(TicketReply reply);
        Task<SupportTicket> GetByGuestTokenAsync(string token);
    }
}
