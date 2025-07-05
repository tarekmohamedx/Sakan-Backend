using Sakan.Infrastructure.Models;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Sakan.Application.Services;
using Sakan.Application.DTOs.User;

namespace Sakan.Hubs
{
    public class ChatHub:Hub
    {
        public ChatHub( IMessageService messageService)
        {
            MessageService = messageService;
        }

        public sakanContext SakanContext { get; }
        public IMessageService MessageService { get; }

        public async Task SendMessage(MessageDto dto)
        {
            var savedMessage = await MessageService.SendMessageAsync(dto);
            await Clients.User(dto.ReceiverID).SendAsync("ReceiveMessage", savedMessage);
            Console.WriteLine("Dto: " + JsonSerializer.Serialize(dto));
        }
    }
}
