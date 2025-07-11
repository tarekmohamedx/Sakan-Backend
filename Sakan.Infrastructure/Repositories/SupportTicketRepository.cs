using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;

namespace Sakan.Infrastructure.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly sakanContext _context;

        public SupportTicketRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket> GetByIdWithRepliesAsync(int ticketId)
        {
            // نستخدم Include لجلب البيانات المرتبطة في طلب واحد
            return await _context.SupportTickets
                .Include(t => t.User) // لجلب اسم المستخدم المسجل
                .Include(t => t.TicketReplies)
                    .ThenInclude(r => r.Author) // لجلب اسم كاتب الرد
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<List<SupportTicket>> GetUserTicketsAsync(string userId)
        {
            return await _context.SupportTickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.LastUpdatedAt ?? t.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddTicketAsync(SupportTicket ticket)
        {
            await _context.SupportTickets.AddAsync(ticket);
        }

        public async Task AddReplyAsync(TicketReply reply)
        {
            await _context.TicketReplies.AddAsync(reply);
        }

        public async Task<List<SupportTicket>> GetAllAsync()
        {
            // هذه الدالة مخصصة للمسؤولين، لذلك سنجلب كل التذاكر
            return await _context.SupportTickets
                .Include(t => t.User) // لجلب معلومات المستخدم المرتبط بالتذكرة (إذا كان مسجلاً)
                .OrderByDescending(t => t.Status == "Open") // عرض التذاكر المفتوحة أولاً
                .ThenByDescending(t => t.Priority == "High") // ثم التذاكر ذات الأولوية العالية
                .ThenByDescending(t => t.LastUpdatedAt ?? t.CreatedAt) // ثم حسب تاريخ آخر تحديث
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<SupportTicket> GetByGuestTokenAsync(string token)
        {
            return await _context.SupportTickets
                .Include(t => t.TicketReplies)
                    .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(t =>t.GuestAccessToken == token && t.UserId == null);
        }
    }
}
