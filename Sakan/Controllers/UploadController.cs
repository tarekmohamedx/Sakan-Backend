using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Application.Services;
using Sakan.Infrastructure.Services;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IImageKitService _imageKitService;

        public UploadController(IImageKitService imageKitService)
        {
            _imageKitService = imageKitService;
        }



        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhotos([FromForm] List<IFormFile> listingPhotos)
        {
            if (listingPhotos == null || listingPhotos.Count == 0)
                return BadRequest("No photos uploaded");

            var photoUrls = new List<string>();
            foreach (var photo in listingPhotos)
            {
                var url = await _imageKitService.UploadImageAsync(photo, "/listings");
                photoUrls.Add(url);
            }

            return Ok(photoUrls);
        }

        [HttpPost("upload-room")]
        public async Task<IActionResult> UploadRoomPhotos([FromForm] List<IFormFile> roomPhotos)
        {
            if (roomPhotos == null || roomPhotos.Count == 0)
                return BadRequest("No room photos uploaded");

            var photoUrls = new List<string>();
            foreach (var photo in roomPhotos)
            {
                var url = await _imageKitService.UploadImageAsync(photo, "/rooms");
                photoUrls.Add(url);
            }

            return Ok(photoUrls);
        }


        [HttpPost("upload-bed")]
        public async Task<IActionResult> UploadBedPhotos([FromForm] List<IFormFile> bedPhotos)
        {
            if (bedPhotos == null || bedPhotos.Count == 0)
                return BadRequest("No bed photos uploaded");

            var photoUrls = new List<string>();
            foreach (var photo in bedPhotos)
            {
                var url = await _imageKitService.UploadImageAsync(photo, "/beds");
                photoUrls.Add(url);
            }

            return Ok(photoUrls);
        }




        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadPhoto([FromForm] IFormFile listingPhotos)
        //{
        //    if (listingPhotos == null || listingPhotos.Length == 0)
        //        return BadRequest("No photo uploaded");

        //    try
        //    {
        //        var url = await _imageKitService.UploadImageAsync(listingPhotos, "/listings");
        //        return Ok(new List<string> { url });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = true, message = ex.Message });
        //    }
        //}


        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file provided");

        //    try
        //    {
        //        var apiKey = "private_YQihV2s3/ZSsza1cLL2B+9TI9Mk=";
        //        var publicKey = "public_2YywJbTJN/tanZ7CP3XsCz+Loec=";

        //        using var stream = file.OpenReadStream();
        //        using var ms = new MemoryStream();
        //        await stream.CopyToAsync(ms);
        //        var base64File = Convert.ToBase64String(ms.ToArray());

        //        using var client = new HttpClient();
        //        var form = new MultipartFormDataContent
        //        {
        //            { new StringContent(base64File), "file" },
        //            { new StringContent(file.FileName), "fileName" }
        //        };

        //        // ✅ Correct Authorization Header
        //        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{publicKey}:{apiKey}"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        //        // ✅ Correct Endpoint
        //        var response = await client.PostAsync("https://upload.imagekit.io/api/v1/files/upload", form);

        //        //// 🔐 Add authorization header
        //        //var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{publicKey}:{apiKey}"));
        //        //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

        //        //var response = await client.PostAsync("https://upload.imagekit.io/api/v1/files/upload", form);
        //        var responseBody = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //            return StatusCode((int)response.StatusCode, responseBody);

        //        return Ok(responseBody);
        //    }
        //    catch (Exception ex)
        //    {
        //        // 👀 Log for development
        //        return StatusCode(500, $"Server error: {ex.Message}");
        //    }
        //}


    }
}
