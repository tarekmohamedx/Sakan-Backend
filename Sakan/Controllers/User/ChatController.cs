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
        public async Task<IActionResult> GetApprovalStatus(int bookingId, string userId, bool isHost)
        {
            var result = await _messageService.GetBookingApprovalStatusAsync(userId, bookingId, isHost);
            return Ok(result);
        }


        //[HttpPost("approve")]
        //public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequest request)
        //{
        //    //var userId = request.UserId ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userId = "6a2f6c64-510c-43a8-a900-ce8ecbb2e889";

        //    // 1. Call core logic
        //    var result = await _messageService.ApproveBookingAsync("6a2f6c64-510c-43a8-a900-ce8ecbb2e889", request.ChatId, request.IsHost);

        //    // 2. Get chat + listing
        //    var chat = await _messageService.GetChatWithListingAsync(request.ChatId);

        //    if (chat?.Listing == null)
        //        return BadRequest("Listing not found");

        //    var listingTitle = chat.Listing.Title ?? "Listing";
        //    var hostId = chat.Listing.HostId;

        //    // 3. Get booking to extract GuestId
        //    var booking = await _messageService.GetLatestActiveBookingAsync(chat.ListingId, userId);
        //    if (booking == null || string.IsNullOrEmpty(booking.GuestId))
        //        return BadRequest("Guest not found");

        //    var guestId = booking.GuestId;

        //    var receiverId = request.IsHost ? guestId : hostId;

        //    // 4. Send SignalR
        //    await _hubContext.Clients.User(receiverId).SendAsync("ReceiveBookingStatusUpdate", new
        //    {
        //        GuestApproved = result.GuestApproved,
        //        HostApproved = result.HostApproved,
        //        Status = result.Status,
        //        ListingTitle = listingTitle,
        //        UserName = result.ApproverName // Already calculated في service
        //    });

        //    return Ok(result);
        //}

        [HttpPost("approve-booking")]
        public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Get booking by ID
            var booking = await _messageService.GetBookingByIdAsync(request.BookingId);
            if (booking == null || booking.Listing == null || booking.Guest == null)
                return BadRequest("Booking or related data not found");

            var listing = booking.Listing;
            var guestId = booking.GuestId!;
            var hostId = listing.HostId;

            // 2. Call approval logic
            var result = await _messageService.ApproveBookingByIdAsync(request.BookingId, guestId, request.IsHost);

            var receiverId = request.IsHost ? guestId : hostId;

            // 3. Send SignalR
            await _hubContext.Clients.User(receiverId).SendAsync("ReceiveBookingStatusUpdate", new
            {
                GuestApproved = result.GuestApproved,
                HostApproved = result.HostApproved,
                Status = result.Status,
                ListingTitle = listing.Title ?? "Listing",
                UserName = result.ApproverName
            });

            return Ok(result);
        }
        [HttpGet("get-booking-id")]
        public async Task<IActionResult> GetBookingIdFromChat(int chatId, string guestId)
        {
            var chat = await _messageService.GetChatWithListingAsync(chatId);

            var booking = await _messageService.GetLatestActiveBookingAsync(chat.ListingId, guestId);

            if (booking == null)
                return NotFound("Booking not found");

            return Ok(booking.Id);
        }


    }
}
