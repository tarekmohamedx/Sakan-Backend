using Sakan.Application.DTOs;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class MessageService : IMessageService
    {
        public MessageService(IMessage _messageRepo)
        {
            MessageRepo = _messageRepo;
        }

        public IMessage MessageRepo { get; }

        public async Task<BookingApprovalResult> ApproveBookingAsync(string userId, int chatId, bool isHost)
        {
            var chat = await MessageRepo.GetChatWithListingAsync(chatId);
            if (chat == null)
                throw new Exception("Chat not found");

            var booking = await MessageRepo.GetLatestActiveBookingAsync(chat.ListingId);
            if (booking == null)
                throw new Exception("No active booking request");

            if (isHost)
                booking.HostApproved = true;
            else
                booking.GuestApproved = true;

            await MessageRepo.SaveChangesAsync();

            // determine status
            string status;
            // ✅ 1. Reject takes priority
            if (booking.HostApproved == false || booking.GuestApproved == false)
            {
                status = "Rejected";
            }
            // ✅ 2. Both approved
            else if (booking.HostApproved == true && booking.GuestApproved == true)
            {
                status = isHost ? "PendingUserBooking" : "GoToPayment";
            }
            // ✅ 3. Pending cases
            else if (booking.HostApproved != true)
            {
                status = isHost ? "Pending" : "PendingHost";
            }
            else if (booking.GuestApproved != true)
            {
                status = isHost ? "PendingGuest" : "Pending";
            }
            else
            {
                status = "Pending";
            }

            return new BookingApprovalResult
            {
                GuestApproved = booking.GuestApproved ?? false,
                HostApproved = booking.HostApproved ?? false,
                Status = status
            };
        }
        

        public Task<Chat> CreateChatIfNotExistsAsync(string senderId, string receiverId, int listingId)
        {
            return MessageRepo.CreateChatIfNotExistsAsync(senderId, receiverId, listingId);
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(int chatId)
        {
            var messages = await MessageRepo.GetMessagesByChatIdAsync(chatId);
            return messages;
        }

        public async Task<IEnumerable<UserChatSummary>> GetUserChatsAsync(string userId)
        {
            var chats = await MessageRepo.GetUserChatsAsync(userId);
            //if (chats == null || !chats.Any())
            //    throw new KeyNotFoundException("No chats found for this user.");

            return chats;
        }

        public async Task<Message> SendMessageAsync(MessageDto dto)
        {
            if (!await MessageRepo.ChatExistsAsync(dto.ChatId.Value))
                throw new KeyNotFoundException("Chat not found");

            var message = new Message
            {
                SenderId = dto.SenderID,
                ReceiverId = dto.ReceiverID,
                Content = dto.Content,
                Timestamp = dto.Timestamp,
                ChatId = dto.ChatId
            };

            await MessageRepo.AddMessageAsync(message);
            await MessageRepo.SaveChangesAsync();
            return message;
        }
    }
}
