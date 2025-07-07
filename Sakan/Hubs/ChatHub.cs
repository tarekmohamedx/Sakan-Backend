using Microsoft.AspNetCore.SignalR;
using Sakan.Application.DTOs.User;
using Sakan.Application.Services;
using Sakan.Infrastructure.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Sakan.Hubs
{
    public class ChatHub:Hub
    {
        public ChatHub( IMessageService messageService)
        {
            MessageService = messageService;
        }
        public IMessageService MessageService { get; }

        public async Task SendMessage(MessageDto dto)
        {
            var savedMessage = await MessageService.SendMessageAsync(dto);
            await Clients.User(dto.ReceiverID).SendAsync("ReceiveMessage", savedMessage);
            Console.WriteLine("Dto: " + JsonSerializer.Serialize(dto));
        }

        public async Task CreateChat(int listingId, string guestId)
        {
            
        }
    }
}
