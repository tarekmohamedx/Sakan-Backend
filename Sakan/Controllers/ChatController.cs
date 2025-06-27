using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Services;

namespace Sakan.Controllers
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
    }
}
