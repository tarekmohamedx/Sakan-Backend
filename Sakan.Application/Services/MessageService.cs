using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
        public MessageService(IMessage _messageRepo, UserManager<ApplicationUser> userManager)
        {
            MessageRepo = _messageRepo;
            UserManager = userManager;
        }

        public IMessage MessageRepo { get; }
        public UserManager<ApplicationUser> UserManager { get; }

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

        //done
        public async Task<BookingApprovalResult> GetBookingApprovalStatusAsync(string userId, int bookingId, bool isHost)
        {
            var booking = await MessageRepo.GetBookingByIdAsync(bookingId);
            if (booking == null)
                throw new Exception("Booking not found");

            if (isHost == false && booking.GuestId != userId)
                throw new UnauthorizedAccessException("User is not part of this booking");

            // Get status
            string status = GetBookingStatus(booking.HostApproved, booking.GuestApproved, isHost, out _);
            var listing = await MessageRepo.GetListingByIdAsync(booking.ListingId ?? 0);
            var user = await UserManager.FindByIdAsync(userId);
            var name = user?.UserName;

            return new BookingApprovalResult
            {
                GuestApproved = booking.GuestApproved ?? false,
                HostApproved = booking.HostApproved ?? false,
                Status = status,
                ListingTitle = listing?.Title ?? "Default Listing Title",
                ApproverName = name,
                ApproverId = userId,
                UserIdToNotify = isHost ? booking.GuestId : listing?.HostId
            };
        }
        //done
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
            return await MessageRepo.GetChatWitIdAsync(chatId);
        }

        public async Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId, string guestId)
        {
            return await MessageRepo.GetLatestActiveBookingAsync(listingId, guestId);
        }

        public async Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId)
        {
            return await MessageRepo.GetLatestActiveBookingAsync(listingId);
        }

        //done
        public async Task<BookingApprovalResult> ApproveBookingByIdAsync(BookingRequest? booking, ApproveBookingRequest ApproveBookingRequest)
        {
            if (booking == null)
                throw new Exception("Booking not found or not active");
            if (string.IsNullOrEmpty(booking.GuestId))
                throw new Exception("No Guest found For this Booking"); 
            
            var listing = await MessageRepo.GetListingByIdAsync(booking.ListingId ?? 0);
            if (listing == null)
                throw new Exception("Listing not found for this booking");
              if (listing.HostId == null) 
                throw new Exception("Host not found for this listing");

              string hostId = listing.HostId;
              string guestId = booking.GuestId;

            string userId;
            if (ApproveBookingRequest.IsHost)
            {
                booking.HostApproved = true;

                userId = hostId;
            }
            else
            {
                booking.GuestApproved = true;
                 userId = guestId;
            }

            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            if (booking.HostApproved == true && booking.GuestApproved == true)
            {
                booking.IsActive = true;
            }

            await MessageRepo.SaveChangesAsync();

            return await GetBookingApprovalStatusAsync(userId, booking.Id, ApproveBookingRequest.IsHost);
           
        }

        public Task<BookingApprovalResult> ApproveBookingAsync(string userId, int chatId, bool isHost)
        {
            throw new NotImplementedException();
        }

        //done
        public async Task<int?> GetBookingIdFromChat(int chatId, string guestId)
        {
            var chat = await MessageRepo.GetChatWitIdAsync(chatId);

            var booking = await MessageRepo.GetLatestActiveBookingAsync(chat.ListingId, guestId);

            return booking.Id;
        }

        //done
        public async Task<BookingRequest?> GetBookingByIdAsync(int bookingId)
        {
            return await MessageRepo.GetBookingByIdAsync(bookingId);
        }
    }
}
