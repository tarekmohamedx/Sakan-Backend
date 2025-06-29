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
