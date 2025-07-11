using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.DTOs;

namespace Sakan.Application.Interfaces
{
    public interface ISupportTicketService
    {
        // إنشاء شكوى جديدة، لاحظ أن userId اختياري (للزوار)
        Task<int> CreateTicketAsync(CreateTicketDto dto, string? userId);

        // إضافة رد على شكوى
        Task AddReplyAsync(int ticketId, CreateReplyDto dto, string authorId, bool isAdmin);

        // جلب تفاصيل شكوى معينة
        Task<TicketDetailsDto> GetTicketByIdAsync(int ticketId, string userId, bool isAdmin);

        // جلب شكاوى مستخدم معين
        Task<List<TicketDetailsDto>> GetUserTicketsAsync(string userId);

        // جلب كل الشكاوى (للأدمن)
        Task<List<TicketDetailsDto>> GetAllTicketsForAdminAsync();

        // تغيير حالة الشكوى (للأدمن)
        Task ChangeTicketStatusAsync(int ticketId, string newStatus);
        Task<TicketDetailsDto> GetTicketByGuestTokenAsync(string token);
        Task AddGuestReplyAsync(string token, CreateReplyDto dto);
    }
}
