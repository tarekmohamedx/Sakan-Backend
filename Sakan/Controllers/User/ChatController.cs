using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.User;
using Sakan.Application.Services;
using Sakan.Domain.Models;
using System.Security.Claims;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public ChatController(IMessageService messageService) =>
            _messageService = messageService;

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

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _messageService.ApproveBookingAsync(userId, request.ChatId, request.IsHost);
            return Ok(result);
        }

    }
}
