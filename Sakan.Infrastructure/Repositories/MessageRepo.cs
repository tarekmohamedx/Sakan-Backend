using Microsoft.EntityFrameworkCore;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Repositories
{
    public class MessageRepo: IMessage
    {
        public MessageRepo(sakanContext _context)
        {
            Context = _context;
        }

        public sakanContext Context { get; }

        public async Task AddMessageAsync(Message message)
        {
            await Context.Messages.AddAsync(message);
        }

        public async Task<bool> ChatExistsAsync(int chatId)
        {
            return await Context.Chats.AnyAsync(c => c.ChatId == chatId);
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId)
        {
            var messages = await Context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<UserChatSummary>> GetUserChatsAsync(string userId)
        {
            var chats = await Context.Chats
                .Include(c => c.Listing)
                    .ThenInclude(l => l.Host)
                .Include(c => c.Messages)
                .Where(c => c.Messages.Any(m => m.SenderId == userId || m.ReceiverId == userId))
                .Select(c => new UserChatSummary
                {
                    ChatId = c.ChatId,
                    ListingId = c.ListingId,
                    ListingTitle = c.Listing.Title,
                    HostName = c.Listing.Host.UserName,

                    UserName = c.Messages
                        .OrderByDescending(m => m.Timestamp)
                        .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                        .Select(otherId => Context.Users
                            .Where(u => u.Id == otherId)
                            .Select(u => u.UserName)
                            .FirstOrDefault()
                        )
                        .FirstOrDefault(),

                    ListingStatus = Context.BookingRequests
    .Where(br => br.ListingId == c.ListingId)
    .OrderByDescending(br => br.FromDate)
    .Select(br =>
        br.HostApproved == true && br.GuestApproved == true ? "Approved" :
        (br.HostApproved == false || br.GuestApproved == false) ? "Rejected" :
        "Pending"
    )
    .FirstOrDefault() ?? "Pending",

                    LastMessage = c.Messages
                        .OrderByDescending(m => m.Timestamp)
                        .Select(m => new LastMessageInfo
                        {
                            Content = m.Content,
                            Timestamp = m.Timestamp ?? DateTime.MinValue,
                            SenderID = m.SenderId,
                            ReceiverID = m.ReceiverId
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return chats;
        }
        public async Task<Chat> CreateChatIfNotExistsAsync(string senderId, string receiverId, int listingId)
        {
            var existingChat = await Context.Chats
                .Where(c => c.ListingId == listingId)
                .Where(c => c.Messages.Any(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId)))
                .FirstOrDefaultAsync();

            if (existingChat != null)
                return existingChat;

            var newChat = new Chat
            {
                ListingId = listingId,
                CreatedAt = DateTime.UtcNow
            };

            Context.Chats.Add(newChat);
            await Context.SaveChangesAsync();

            return newChat;
        }


        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        //done
     public async Task<Chat?> GetChatWitIdAsync(int chatId)
        { 
             return await Context.Chats.FirstOrDefaultAsync(c => c.ChatId == chatId);
        }

        //done
        public async Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId)
        {
            return await Context.BookingRequests
                .Where(br => br.ListingId == listingId && br.IsActive)
                .OrderByDescending(br => br.CreatedAt)
                .FirstOrDefaultAsync();
        }

        //done
        public async Task<BookingRequest?> GetLatestActiveBookingAsync(int listingId, string guestId)
        {
             return await Context.BookingRequests
                .Where(br => br.ListingId == listingId && br.GuestId == guestId && br.IsActive)
                .OrderByDescending(br => br.CreatedAt)
                .FirstOrDefaultAsync();
        }

     public async Task<string?> GetGuestIdFromChat(int chatId)
{
    var listingId = await Context.Chats
        .Where(c => c.ChatId == chatId)
        .Select(c => c.ListingId)
        .FirstOrDefaultAsync();

    if (listingId == 0) return null;

    var guestId = await Context.BookingRequests
        .Where(b => b.ListingId == listingId && b.IsActive)
        .OrderByDescending(b => b.FromDate)
        .Select(b => b.GuestId)
        .FirstOrDefaultAsync();

    return guestId;
}

        //done review
        public async Task<BookingRequest?> GetBookingByIdAsync(int bookingId)
        {
            //return await Context.BookingRequests
            //    .Include(b => b.Guest)
            //    .Include(b => b.Listing)
            //        .ThenInclude(l => l.Host)
            //    .Include(b => b.Listing)
            //        .ThenInclude(l => l.Chats)
            //    .FirstOrDefaultAsync(b => b.Id == bookingId);

            return await Context.BookingRequests
                .FirstAsync(b => b.Id == bookingId);
        }
        //done 
        public async Task<Listing> GetListingByIdAsync(int listingId)
        {
            return await Context.Listings
                .Where(l => l.Id == listingId && l.IsActive && !l.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetGuestIdByChatId(int chatId)
        {
            var listingId = await Context.Chats
              .Where(c => c.ChatId == chatId)
              .Select(c => c.ListingId)
              .FirstOrDefaultAsync();

            if (listingId == 0) return null;

            var guestId = await Context.BookingRequests
                .Where(b => b.ListingId == listingId && b.IsActive)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => b.GuestId)
                .FirstOrDefaultAsync();

            return guestId;
        }

     



    }
}
