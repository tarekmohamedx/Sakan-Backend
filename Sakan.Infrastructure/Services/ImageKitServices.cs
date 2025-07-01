using Imagekit.Sdk;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services
{
    public class ImageKitServices
    {
        private readonly ImagekitClient _imagekit;

        public ImageKitServices(IConfiguration config)
        {
            _imagekit = new ImagekitClient(
                publicKey: config["ImageKit:PublicKey"],
                privateKey: config["ImageKit:PrivateKey"],
                urlEndPoint: config["ImageKit:UrlEndpoint"]
            );
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var byteArray = ms.ToArray();

            var uploadRequest = new FileCreateRequest
            {
                file = byteArray,
                fileName = file.FileName
            };

            var result = await _imagekit.UploadAsync(uploadRequest);
            return result?.url;
        }
    }

}
