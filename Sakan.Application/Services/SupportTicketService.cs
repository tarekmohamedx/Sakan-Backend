using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Interfaces;
using Sakan.Domain.IUnitOfWork;
using Sakan.Domain.Models;

namespace Sakan.Application.Services
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly ISupportTicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupportTicketService(ISupportTicketRepository ticketRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateTicketAsync(CreateTicketDto dto, string? userId)
        {
            var ticket = new SupportTicket
            {
                Subject = dto.Subject,
                Description = dto.Description,
                Category = dto.Category,
                BookingId = dto.BookingId,
                Status = "Open", // القيمة الافتراضية
                Priority = "Normal",
                CreatedAt = DateTime.UtcNow,
            };

            if (userId == null)
            {
                ticket.UserId = null;
                ticket.GuestName = dto.GuestName;
                ticket.GuestEmail = dto.GuestEmail;
                ticket.GuestAccessToken = Guid.NewGuid().ToString("N");
            }
            else
            {
                ticket.UserId = userId;
            }

            await _ticketRepository.AddTicketAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            if (userId == null)
            {
                var ticketLink = $"https://your-frontend-url.com/support/guest-ticket/{ticket.GuestAccessToken}";
                var emailBody = $"Thank you for contacting support. You can view and reply to your ticket using this private link: {ticketLink}";
                //await _emailService.SendEmailAsync(ticket.GuestEmail, $"Support Ticket #{ticket.Id}", emailBody);
            }

            return ticket.Id;
        }

        public async Task AddReplyAsync(int ticketId, CreateReplyDto dto, string authorId, bool isAdmin)
        {
            // يجب التحقق أولاً إذا كان للـ authorId صلاحية الرد على هذه الشكوى
            var ticket = await _ticketRepository.GetByIdWithRepliesAsync(ticketId);
            if (ticket == null || (ticket.UserId != authorId && !isAdmin)) // IsAdmin هي دالة للتحقق من صلاحيات المستخدم
            {
                throw new UnauthorizedAccessException("You are not authorized to reply to this ticket.");
            }

            var reply = new TicketReply
            {
                SupportTicketId = ticketId,
                AuthorId = authorId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddReplyAsync(reply);
            ticket.LastUpdatedAt = DateTime.UtcNow; // تحديث وقت آخر رد
            ticket.Status = "InProgress"; // تغيير الحالة
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TicketDetailsDto> GetTicketByIdAsync(int ticketId, string userId, bool isAdmin)
        {
            var ticket = await _ticketRepository.GetByIdWithRepliesAsync(ticketId);
            if (ticket == null || (ticket.UserId != userId && !isAdmin))
            {
                throw new UnauthorizedAccessException("Access denied.");
            }

            // هنا يمكن استخدام AutoMapper لتحويل ticket إلى TicketDetailsDto
            return _mapper.Map<TicketDetailsDto>(ticket);
        }

        public async Task<List<TicketDetailsDto>> GetAllTicketsForAdminAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            // نفترض أن لديك AutoMapper Profile لتحويل SupportTicket إلى TicketSummaryDto
            return _mapper.Map<List<TicketDetailsDto>>(tickets);
        }

        public async Task ChangeTicketStatusAsync(int ticketId, string newStatus)
        {
            // لا داعي للتحقق من صلاحيات الأدمن هنا، لأن الـ Controller سيقوم بذلك
            var ticket = await _ticketRepository.GetByIdWithRepliesAsync(ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            ticket.Status = newStatus;
            ticket.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TicketDetailsDto>> GetUserTicketsAsync(string userId)
        {
            var tickets = await _ticketRepository.GetUserTicketsAsync(userId);
            return _mapper.Map<List<TicketDetailsDto>>(tickets);
        }

        public async Task<TicketDetailsDto> GetTicketByGuestTokenAsync(string token)
        {
            var ticket = await _ticketRepository.GetByGuestTokenAsync(token);
            if (ticket == null) throw new KeyNotFoundException("Ticket not found.");
            return _mapper.Map<TicketDetailsDto>(ticket);
        }

        public async Task AddGuestReplyAsync(string token, CreateReplyDto dto)
        {
            var ticket = await _ticketRepository.GetByGuestTokenAsync(token);
            if (ticket == null) throw new KeyNotFoundException("Ticket not found.");

            var reply = new TicketReply
            {
                SupportTicketId = ticket.Id,
                AuthorId = null, // الرد من زائر، لا يوجد AuthorId
                Content = $"[Guest Reply from: {ticket.GuestEmail}]\n\n{dto.Content}", // إضافة علامة مميزة
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddReplyAsync(reply);
            ticket.LastUpdatedAt = DateTime.UtcNow;
            ticket.Status = "InProgress";
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
