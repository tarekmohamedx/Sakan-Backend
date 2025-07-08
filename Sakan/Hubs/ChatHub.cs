using Microsoft.AspNetCore.SignalR;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using Sakan.Application.Services;
using Sakan.Infrastructure.Models;
using Sakan.Infrastructure.Services.User;
using System.Security.Claims;
using System.Text.Json;

namespace Sakan.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(IMessageService messageService, IBookingRequestService bookingRequestService)
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
            public async Task ChatWithHost(int listingId, string guestId)
            {
                var latestBookingRequest = await BookingRequestService.GetLatestBookingRequestAsync(listingId, guestId);

                if (latestBookingRequest != null)
                {
                var message = "chatting with host now on a listing";

                    await Clients.User(guestId).SendAsync("ReceiveBookingRequestInfo", new
                    {
                        Message = message,
                        Request = latestBookingRequest
                    });
                }
            
        }

        public async Task NotifyBookingStatusChanged(string receiverId, BookingApprovalResult result)
        {
            await Clients.User(receiverId).SendAsync("ReceiveBookingStatusUpdate", result);
        }


    }
}