using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces.User
{
    public interface IImageKitService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);

    }
}
