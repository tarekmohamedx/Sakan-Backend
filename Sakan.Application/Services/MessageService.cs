using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs.User;
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

            string? guestId;

            // لو المستخدم Host، نجيب الـ guest من أحدث booking
            if (isHost)
            {
                var latestBooking = await MessageRepo.GetLatestActiveBookingAsync(chat.ListingId);
                if (latestBooking == null)
                    throw new Exception("No booking found");

                guestId = latestBooking.GuestId;
                if (string.IsNullOrEmpty(guestId))
                    throw new Exception("Guest ID not found in booking");

                // نستخدم الـ latestBooking بدل ما ننده نفس الـ method تاني
                var booking = latestBooking;

                if (booking.HostApproved == false)
                    throw new Exception("Host already rejected this booking");
                if (booking.GuestApproved == false)
                    throw new Exception("Guest already rejected this booking");

                if (booking.HostApproved == null)
                    booking.HostApproved = true;

                await MessageRepo.SaveChangesAsync();

                return GenerateResult(booking, chat, isHost);
            }

            // لو المستخدم هو الضيف
            guestId = userId;
            var bookingForGuest = await MessageRepo.GetLatestActiveBookingAsync(chat.ListingId, guestId);

            if (bookingForGuest == null)
                throw new Exception("No active booking request");

            //if (bookingForGuest.HostApproved == false)
            //    throw new Exception("Host already rejected this booking");
            //if (bookingForGuest.GuestApproved == false)
            //    throw new Exception("Guest already rejected this booking");

            if (bookingForGuest.GuestApproved == null)
                bookingForGuest.GuestApproved = true;

            await MessageRepo.SaveChangesAsync();

            return GenerateResult(bookingForGuest, chat, isHost);
        }
        private BookingApprovalResult GenerateResult(BookingRequest booking, Chat chat, bool isHost)
        {
            string status;
            var host = booking.HostApproved;
            var guest = booking.GuestApproved;

            if (host == true && guest == true)
                status = isHost ? "PendingUserBooking" : "GoToPayment";
            else if (host == false)
                status = "RejectedByHost";
            else if (guest == false)
                status = "RejectedByGuest";
            else if (host == true)
                status = "ApprovedByHost";
            else if (guest == true)
                status = "ApprovedByGuest";
            else
                status = "Pending";

            return new BookingApprovalResult
            {
                GuestApproved = guest ?? false,
                HostApproved = host ?? false,
                Status = status,
                ListingTitle = chat.Listing?.Title ?? "Listing",
                ApproverName = isHost ? chat.Listing?.Host?.UserName : booking.Guest?.UserName ?? "User"
            };
        }


        public async Task<BookingApprovalResult> GetBookingApprovalStatusAsync(string userId, int chatId, bool isHost)
        {
            var chat = await MessageRepo.GetChatWithListingAsync(chatId);
            if (chat == null)
                throw new Exception("Chat not found");

            var booking = await MessageRepo.GetLatestActiveBookingAsync(chat.ListingId, userId);
            if (booking == null)
                throw new Exception("No active booking request");

            System.Diagnostics.Debug.WriteLine($"BookingRequestId = {booking?.Id}, GuestApproved = {booking?.GuestApproved}, HostApproved = {booking?.HostApproved}, IsActive = {booking?.IsActive}");

            // Get status
            string status = GetBookingStatus(booking.HostApproved, booking.GuestApproved, isHost, out _);

            return new BookingApprovalResult
            {
                GuestApproved = booking.GuestApproved ?? false,
                HostApproved = booking.HostApproved ?? false,
                Status = status
            };
        }
        private string GetBookingStatus(bool? host, bool? guest, bool isHost, out string logicalStatus)
        {
            if (host == null && guest == null)
                logicalStatus = "Pending";
            else if (host == false)
                logicalStatus = "RejectedByHost";
            else if (guest == false)
                logicalStatus = "RejectedByGuest";
            else if (host == true && guest == true)
                logicalStatus = isHost ? "PendingUserBooking" : "GoToPayment";
            else if (host == true)
                logicalStatus = "ApprovedByHost";
            else if (guest == true)
                logicalStatus = "ApprovedByGuest";
            else
                logicalStatus = "Pending";

            return logicalStatus;
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

        public async Task<Chat> GetChatWithListingAsync(int chatId)
        {
            return await MessageRepo.GetChatWithListingAsync(chatId);
        }

        public async Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId, string guestId)
        {
            return await MessageRepo.GetLatestActiveBookingAsync(listingId, guestId);
        }

        public async Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId)
        {
            return await MessageRepo.GetLatestActiveBookingAsync(listingId);
        }

        public async Task<BookingApprovalResult> ApproveBookingByIdAsync(int bookingId, string approverId, bool isHost)
        {
            var booking = await MessageRepo.GetBookingByIdAsync(bookingId);
            if (booking == null || !booking.IsActive)
                throw new Exception("Booking not found or not active");

            if (isHost)
                booking.HostApproved = true;
            else
                booking.GuestApproved = true;

            await MessageRepo.SaveChangesAsync();

            var status = booking.HostApproved == true && booking.GuestApproved == true
                ? "GoToPayment"
                : isHost ? "PendingGuest" : "PendingHost";

            return new BookingApprovalResult
            {
                GuestApproved = booking.GuestApproved ?? false,
                HostApproved = booking.HostApproved ?? false,
                Status = status,
                ListingTitle = booking.Listing?.Title ?? "Listing",
                ApproverName = isHost ? booking.Listing?.Host?.UserName : booking.Guest?.UserName ?? "User"
            };
        }

        public async Task<BookingRequest?> GetBookingByIdAsync(int bookingId)
        {
            return await MessageRepo.GetBookingByIdAsync(bookingId);
        }
    }
}
