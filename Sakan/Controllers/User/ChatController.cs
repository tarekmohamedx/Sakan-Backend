using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sakan.Application.DTOs.User;
using Sakan.Application.Services;
using Sakan.Domain.Models;
using Sakan.Hubs;
using System.Security.Claims;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        public UserManager<ApplicationUser> UserManager { get; }

        public ChatController(
            IMessageService messageService,
            IHubContext<ChatHub> hubContext,
            UserManager<ApplicationUser> userManager
                            )
        {
            _messageService = messageService;
            _hubContext = hubContext;
            UserManager = userManager;
        }
            

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto dto)
        {
            try
            {
                var message = await _messageService.SendMessageAsync(dto);
                return Ok(message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("history/{chatId}")]
        public async Task<IActionResult> GetChatHistory(int chatId)
        {
            var messages = await _messageService.GetChatHistoryAsync(chatId);
            if (!messages.Any())
            {
                return NotFound("No messages found for this chat.");
            }
            return Ok(messages);
        }

        [HttpGet("user/{userId}/chats")]
        public async Task<IActionResult> GetUserChats(string userId)
        {
            try
            {
                var chats = await _messageService.GetUserChatsAsync(userId);
                return Ok(chats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CreateChatIfNotExists")]
        public async Task<ActionResult<UserChatSummary>> CreateChatIfNotExists([FromBody] CreateChatRequestDTO request)
        {
            var chat = await _messageService.CreateChatIfNotExistsAsync(request.SenderId, request.ReceiverId, request.ListingId);
            return Ok(chat);
        }

        //done
        [HttpGet("approval-status")]
        public async Task<IActionResult> GetApprovalStatus(int bookingId, string userId, bool isHost)
        {
            var result = await _messageService.GetBookingApprovalStatusAsync(userId, bookingId, isHost);
            return Ok(result);
        }

        //done
        [HttpPost("approve-booking")]
        public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Get booking by ID
            var booking = await _messageService.GetBookingByIdAsync(request.BookingId);
            if (booking == null)
                return BadRequest("Booking or related data not found");


            // 2. Call approval logic
            var result = await _messageService.ApproveBookingByIdAsync(booking, request);



            // 3. Send SignalR
            await _hubContext.Clients.User(result.UserIdToNotify).SendAsync("ReceiveBookingStatusUpdate", new
            {
                GuestApproved = result.GuestApproved,
                HostApproved = result.HostApproved,
                Status = result.Status,
                ListingTitle = result.ListingTitle ?? "Default Listing Title",
                UserName = result.ApproverName
            });

            return Ok(result);
        }

        //done
        [HttpGet("get-booking-id")]
        public async Task<IActionResult> GetBookingIdFromChat(int chatId, string guestId)
        {
            var bookingId = await _messageService.GetBookingIdFromChat(chatId, guestId);
            if (bookingId == null)
            {
                return NotFound("Booking ID not found for the specified chat and guest.");
            }
            return Ok(bookingId);
        }


    }
}
