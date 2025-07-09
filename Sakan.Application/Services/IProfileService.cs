using Sakan.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public interface IProfileService
    {
        UserProfileDTO GetUserprofilebyid(string id);

    }
}
