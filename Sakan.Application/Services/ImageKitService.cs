using Imagekit.Sdk;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sakan.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class ImageKitService : IImageKitService
    {
        private readonly IConfiguration _configuration;
        private readonly ImagekitClient _imagekit;

        public ImageKitService(IConfiguration configuration, ImagekitClient imagekit)
        {
            _configuration = configuration;
            _imagekit = new ImagekitClient(
                _configuration["ImageKit:PublicKey"],
                _configuration["ImageKit:PrivateKey"],
                _configuration["ImageKit:UrlEndpoint"]
            );

        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("File size must be less than 5MB");

            // Validate extension
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Only .png, .jpg, .jpeg formats are allowed");

            using var stream = file.OpenReadStream();
            var fileCreateRequest = new FileCreateRequest
            {
                file = stream,
                fileName = Guid.NewGuid().ToString() + extension,
                folder = folder
            };

            var result = await _imagekit.UploadAsync(fileCreateRequest);
            return result.url;
        }
    }
}
