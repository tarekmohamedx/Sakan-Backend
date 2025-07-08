using Sakan.Application.DTOs.User;
using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(MessageDto dto);
        Task<IEnumerable<Message>> GetChatHistoryAsync(int chatId);
        Task<IEnumerable<UserChatSummary>> GetUserChatsAsync(string userId);
        Task<Chat> CreateChatIfNotExistsAsync(string senderId, string receiverId, int listingId);
        Task<BookingApprovalResult> ApproveBookingAsync(string userId, int chatId, bool isHost);
        Task<BookingApprovalResult> GetBookingApprovalStatusAsync(string userId, int chatId, bool isHost);
        Task<Chat> GetChatWithListingAsync(int chatId);
        Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId, string guestId);
        Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId);
        Task<BookingApprovalResult> ApproveBookingByIdAsync(int bookingId, string approverId, bool isHost);
        Task<BookingRequest?> GetBookingByIdAsync(int bookingId);
        Task<string?> GetGuestIdFromChat(int chatId);
    }
}
