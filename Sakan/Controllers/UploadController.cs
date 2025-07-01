using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services;
using System.Text;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ImageKitService _imageKitService;

        public UploadController(ImageKitService imageKitService)
        {
            _imageKitService = imageKitService;
        }

        //[HttpPost]
        //[RequestSizeLimit(10 * 1024 * 1024)] // 10 MB max
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded");

        //    var imageUrl = await _imageKitService.UploadImageAsync(file, "listings");

        //    return Ok(new { url = imageUrl });
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    var apiKey = "private_YQihV2s3/ZSsza1cLL2B+9TI9Mk=";
        //    var publicKey = "public_2YywJbTJN/tanZ7CP3XsCz+Loec=";

        //    var bytes = new byte[file.Length];
        //    using var stream = file.OpenReadStream();
        //    await stream.ReadAsync(bytes, 0, (int)file.Length);

        //    var base64File = Convert.ToBase64String(bytes);

        //    using var client = new HttpClient();
        //    var formData = new MultipartFormDataContent
        //    {
        //        { new StringContent(base64File), "file" },
        //        { new StringContent(file.FileName), "fileName" },
        //        { new StringContent(publicKey), "publicKey" }
        //    };

        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
        //        "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{publicKey}:{apiKey}")));

        //    var response = await client.PostAsync("https://upload.imagekit.io/api/v1/files/upload", formData);
        //    var result = await response.Content.ReadAsStringAsync();

        //    return Content(result, "application/json");
        //}


        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            try
            {
                var apiKey = "private_YQihV2s3/ZSsza1cLL2B+9TI9Mk=";
                var publicKey = "public_2YywJbTJN/tanZ7CP3XsCz+Loec=";

                using var stream = file.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var base64File = Convert.ToBase64String(ms.ToArray());

                using var client = new HttpClient();
                var form = new MultipartFormDataContent
                {
                    { new StringContent(base64File), "file" },
                    { new StringContent(file.FileName), "fileName" }
                };

                // 🔐 Add authorization header
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{publicKey}:{apiKey}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var response = await client.PostAsync("https://upload.imagekit.io/api/v1/files/upload", form);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, responseBody);

                return Ok(responseBody);
            }
            catch (Exception ex)
            {
                // 👀 Log for development
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }


    }
}
