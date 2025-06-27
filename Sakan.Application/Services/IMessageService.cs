using Sakan.Application.DTOs;
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
    }
}
