using Sakan.Infrastructure.Models;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Sakan.Application.DTOs;

namespace Sakan.Hubs
{
    public class ChatHub:Hub
    {
        public ChatHub(sakanContext sakanContext)
        {
            SakanContext = sakanContext;
        }

        public sakanContext SakanContext { get; }

        public async Task SendMessage(MessageDto dto)
        {
            await Clients.User(dto.ReceiverID).SendAsync("ReceiveMessage", dto);
            Console.WriteLine("Dto: " + JsonSerializer.Serialize(dto));
            //await Clients.All.SendAsync("ReceiveMessage", dto);
        }
    }
}
