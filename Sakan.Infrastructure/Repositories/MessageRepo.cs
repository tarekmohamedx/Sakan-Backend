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
            return await Context.Chats
           .Where(chat => Context.Messages
               .Any(m => m.ChatId == chat.ChatId &&
                        (m.SenderId == userId || m.ReceiverId == userId)))
           .Select(chat => new UserChatSummary
           {
               ChatId = chat.ChatId,
               ListingId = chat.ListingId,
               ListingTitle = "Mansoura Apartmentt",
               UserName = Context.Users
                .Where(u =>
                    u.Id == Context.Messages
                        .Where(m => m.ChatId == chat.ChatId)
                        .OrderByDescending(m => m.Timestamp)
                        .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                        .FirstOrDefault()
                )
                .Select(u => u.UserName)
                .FirstOrDefault(),
               LastMessage = Context.Messages
                   .Where(m => m.ChatId == chat.ChatId)
                   .OrderByDescending(m => m.Timestamp)
                   .Select(m => new LastMessageInfo
                   {
                       Content = m.Content,
                       Timestamp = m.Timestamp.Value,
                       SenderID = m.SenderId,
                       ReceiverID = m.ReceiverId
                   })
                   .FirstOrDefault()
           })
           .ToListAsync();
        }

        public async Task<Chat> CreateChatIfNotExistsAsync(string senderId, string receiverId, int listingId)
        {
    
            var existingChat = await Context.Chats
                .FirstOrDefaultAsync(c => c.ListingId == listingId &&
                    Context.Messages.Any(m =>
                        m.ChatId == c.ChatId &&
                        ((m.SenderId == senderId && m.ReceiverId == receiverId) ||
                         (m.SenderId == receiverId && m.ReceiverId == senderId)))
                );

            if (existingChat != null)
                return existingChat;

            var possibleChat = await Context.Chats
                .FirstOrDefaultAsync(c => c.ListingId == listingId);

            if (possibleChat != null)
                return possibleChat;

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
    }
}
