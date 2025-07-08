using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sakan.Infrastructure.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sakan.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;
    private readonly sakanContext _context;

    public AiController(IHttpClientFactory httpClientFactory, IOptions<OpenAIOptions> options, sakanContext context)
    {
        _httpClient = httpClientFactory.CreateClient();
        _options = options.Value;
        _context = context;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequestDto request)
    {
        var contextText = await BuildPlatformContextAsync();

        var openAiRequest = new
        {
            model = "gpt-4o-mini",
            messages = new List<object>
        {
            new
            {
                role = "system",
                //content = "You are a helpful and intelligent assistant for the Sakan rental platform. Format your responses clearly using bullet points and real line breaks for readability.\n\n" + contextText
                content = "You are a helpful and intelligent assistant for the Sakan rental platform. Here's a summary of current data:\n\n" + contextText
                //content = "You are a helpful and intelligent assistant for the Sakan rental platform... Format your responses using bullet points (`•`) and line breaks (`\\n`). Wrap each listing in a separate paragraph.\n\n" + contextText

            }
        }.Concat(request.Messages.Select(m => new { role = m.Role, content = m.Content }))
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(openAiRequest), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(responseJson);

        var parsed = JsonDocument.Parse(responseJson);
        var reply = parsed.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return Ok(new { reply });
    }


    private async Task<string> BuildPlatformContextAsync()
    {
        var listings = await _context.Listings
            .Include(l => l.Rooms).ThenInclude(r => r.Beds)
            .Include(l => l.ListingPhotos)
            .ToListAsync();

        var reviews = await _context.Reviews
            .Include(r => r.Listing)
            .Include(r => r.Reviewer)
            .ToListAsync();

        var bookings = await _context.Bookings
            .Include(b => b.Listing)
            .Include(b => b.Room)
            .Include(b => b.Bed)
            .ToListAsync();

        var tickets = await _context.SupportTickets
            .Include(t => t.User)
            .ToListAsync();

        var sb = new StringBuilder();

        // Listings
        sb.AppendLine("📋 **Top Listings Summary:**\n");
        foreach (var l in listings.Take(10))
        {
            var bedCount = l.Rooms.SelectMany(r => r.Beds).Count();
            sb.AppendLine($"🏠 **{l.Title}** \n");
            sb.AppendLine();
            sb.AppendLine($"   • Location: {l.Governorate}, {l.District} \n");
            sb.AppendLine();
            sb.AppendLine($"   • Price/Month: {(l.PricePerMonth.HasValue ? $"{l.PricePerMonth.Value:N0} EGP" : "N/A")} \n");
            sb.AppendLine();
            sb.AppendLine($"   • Rooms: {l.Rooms.Count}, Beds: {bedCount} \n");
            sb.AppendLine();
        }

        // Reviews
        sb.AppendLine("⭐ **Latest Reviews:**\n");
        foreach (var r in reviews.Take(10))
        {
            sb.AppendLine($"👤 {r.Reviewer?.UserName ?? "Unknown"} → \"{r.Listing?.Title ?? "N/A"}\"");
            sb.AppendLine($"   • Rating: {r.Rating}/5");
            if (!string.IsNullOrWhiteSpace(r.Comment))
                sb.AppendLine($"   • Comment: \"{r.Comment}\"");
            sb.AppendLine();
        }

        // Bookings
        sb.AppendLine("📦 **Recent Bookings:**\n");
        foreach (var b in bookings.Take(10))
        {
            sb.AppendLine($"🏷️ Listing: {b.Listing?.Title ?? "N/A"}");
            sb.AppendLine($"   • Room: {b.Room?.Name}, Bed: {b.Bed?.Label}");
            sb.AppendLine($"   • Payment: {b.PaymentStatus}, {b.FromDate:yyyy-MM-dd} → {b.ToDate:yyyy-MM-dd}");
            sb.AppendLine();
        }

        // Support Tickets
        sb.AppendLine("🎫 **Support Tickets:**\n");
        foreach (var t in tickets.Take(10))
        {
            sb.AppendLine($"📨 From: {t.User?.UserName ?? "Unknown"}");
            sb.AppendLine($"   • Subject: {t.Subject}");
            sb.AppendLine($"   • Status: {t.Status}");
            sb.AppendLine();
        }

        return sb.ToString();
    }


    public class ChatRequestDto
    {
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        public string Role { get; set; } // user | assistant | system
        public string Content { get; set; }
    }

    public class OpenAIOptions
    {
        public string ApiKey { get; set; }
    }
}
