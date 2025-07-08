using Microsoft.AspNetCore.SignalR;
using Sakan.Application.Services;
using Sakan.Application.DTOs.User;

namespace Sakan.Hubs
{
    public class ChatHub:Hub
    {
        public ChatHub( IMessageService messageService, IBookingRequestService bookingRequestService)
        {
            MessageService = messageService;
            BookingRequestService = bookingRequestService;
        }
        public IMessageService MessageService { get; }
        public IBookingRequestService BookingRequestService { get; }

        public async Task SendMessage(MessageDto dto)
        {
            var savedMessage = await MessageService.SendMessageAsync(dto);
            await Clients.User(dto.ReceiverID).SendAsync("ReceiveMessage", savedMessage);
            Console.WriteLine("Dto: " + JsonSerializer.Serialize(dto));
        }
    }
}
