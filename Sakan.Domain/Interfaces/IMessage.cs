using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Interfaces
{
    public interface IMessage
    {
        Task<bool> ChatExistsAsync(int chatId);
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId);
        Task SaveChangesAsync();
        Task<IEnumerable<UserChatSummary>> GetUserChatsAsync(string userId);
        Task<Chat> CreateChatIfNotExistsAsync(string senderId, string receiverId, int listingId);

    }
}
