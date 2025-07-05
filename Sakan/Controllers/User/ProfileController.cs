using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService profileService;

        public ProfileController(IProfileService profileService)
        {
            this.profileService = profileService;
        }

        [HttpGet]
        public IActionResult GetUserprofilebyid(string id)
        {
            var profileuser = profileService.GetUserprofilebyid(id); 
            if (profileuser == null)
                return NotFound(); 


            return Ok(profileuser); 
        }
    }
}
