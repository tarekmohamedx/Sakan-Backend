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

        public ChatController(IMessageService messageService,
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

        [HttpGet("approval-status")]
        public async Task<IActionResult> GetApprovalStatus(int chatId, string userId, bool isHost)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _messageService.GetBookingApprovalStatusAsync(userId, chatId, isHost);
            return Ok(result);

        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Call core logic
            var result = await _messageService.ApproveBookingAsync("6a2f6c64-510c-43a8-a900-ce8ecbb2e889", request.ChatId, request.IsHost);

            // 2. Get chat + listing + sender
            var chat = await _messageService.GetChatWithListingAsync(request.ChatId);

            if (chat?.Listing == null)
                return BadRequest("Listing not found");

            var listingTitle = chat.Listing.Title ?? "Listing";

            // 3. Identify host and guest
            var hostId = chat.Listing.HostId;
            var guestId = chat.Messages.FirstOrDefault()?.SenderId;

            if (guestId == null)
                return BadRequest("Guest not found");

            var receiverId = request.IsHost ? guestId : hostId;

            // 4. Sender name
            //var userName = await UserManager.GetUserNameAsync(User);

            // 5. Send SignalR
            await _hubContext.Clients.User(receiverId).SendAsync("ReceiveBookingStatusUpdate", new
            {
                GuestApproved = result.GuestApproved,
                HostApproved = result.HostApproved,
                Status = result.Status,
                ListingTitle = listingTitle,
                UserName = "Tarek Mohamed"
            });

            return Ok(result);
        }

    }
}
