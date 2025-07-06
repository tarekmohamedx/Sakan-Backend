using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Sakan.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AiController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost("generate-response")]
        public async Task<IActionResult> GenerateResponse([FromBody] AiPromptDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are a professional and polite customer service assistant for a room rental platform." },
                new { role = "user", content = $"Please write a friendly response to: '{dto.InputText}'" }
            }
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { error = "OpenAI API call failed", details = error });
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            // Now check if "choices" exists before accessing
            if (!json.TryGetProperty("choices", out JsonElement choices) || choices.GetArrayLength() == 0)
            {
                return BadRequest(new { error = "No choices returned from OpenAI" });
            }

            var reply = choices[0].GetProperty("message").GetProperty("content").GetString();

            return Ok(new { response = reply });

        }
    }

    public class AiPromptDto
    {
        public string InputText { get; set; }
    }

}
