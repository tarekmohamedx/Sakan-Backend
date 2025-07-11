using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;

namespace Sakan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportTicketsController : ControllerBase
    {
        private readonly ISupportTicketService _ticketService;

        public SupportTicketsController(ISupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // POST: api/support-tickets
        // يمكن استدعاؤه من الزوار أو المستخدمين المسجلين
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            // التحقق إذا كان المستخدم مسجلاً
            string? userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var ticketId = await _ticketService.CreateTicketAsync(dto, userId);
            return Ok(new { TicketId = ticketId });
        }

        // GET: api/support-tickets/mytickets
        [HttpGet("mytickets")]
        [Authorize] // للمستخدمين المسجلين فقط
        public async Task<IActionResult> GetMyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = await _ticketService.GetUserTicketsAsync(userId);
            return Ok(tickets);
        }

        // GET: api/support-tickets/{ticketId}
        [HttpGet("{ticketId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTicket(int ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin"); // التحقق إذا كان المستخدم أدمن

            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId, userId, isAdmin);
                return Ok(ticket);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // POST: api/support-tickets/{ticketId}/replies
        [HttpPost("{ticketId:int}/replies")]
        [Authorize]
        public async Task<IActionResult> AddReply(int ticketId, [FromBody] CreateReplyDto dto)
        {
            var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            try
            {
                await _ticketService.AddReplyAsync(ticketId, dto, authorId,isAdmin);
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message);
            }
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")] // <-- حماية الـ Endpoint للأدمن فقط
        public async Task<IActionResult> GetAllTicketsForAdmin()
        {
            var tickets = await _ticketService.GetAllTicketsForAdminAsync();
            return Ok(tickets);
        }

        // PUT: api/support-tickets/{ticketId}/status (خاص بالأدمن)
        [HttpPut("{ticketId:int}/status")]
        [Authorize(Roles = "Admin")] // <-- حماية الـ Endpoint للأدمن فقط
        public async Task<IActionResult> ChangeStatus(int ticketId, [FromBody] ChangeStatusRequest request)
        {
            try
            {
                await _ticketService.ChangeTicketStatusAsync(ticketId, request.NewStatus);
                return NoContent(); // 204 No Content هو الرد المناسب لعملية تحديث ناجحة
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("guest/{token}")]
        public async Task<IActionResult> GetGuestTicket(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Access token is required.");
            }
            try
            {
                var ticket = await _ticketService.GetTicketByGuestTokenAsync(token);
                return Ok(ticket);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message); // 403 Forbidden
            }
        }

        [HttpPost("guest/{token}/replies")]
        public async Task<IActionResult> AddGuestReply(string token, [FromBody] CreateReplyDto dto)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Access token is required.");
            }
            try
            {
                await _ticketService.AddGuestReplyAsync(token, dto);
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message);
            }
        }
        public class ChangeStatusRequest
        {
            public string NewStatus { get; set; }
        }
    }
}
