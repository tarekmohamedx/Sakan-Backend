using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services;
using Sakan.Domain.Models;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITestService testService;
        private readonly IProfileService ProfileService;
        private readonly RoleManager<IdentityRole> roleManager;
        public TestController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITestService testService, IProfileService profileService)
        {
            this.userManager = userManager;

            this.roleManager = roleManager;
            this.testService = testService;
            ProfileService = profileService;
        }


        [HttpGet]
        public  IActionResult getallroles()
        {
            var roles = roleManager.Roles.ToList(); 
            return Ok(roles);
        }

        [HttpGet("test")]
        public IActionResult getalltests()
        {
            var tests = testService.testsnames(); 
            return Ok(tests); 
        }

        [HttpGet("profile/{userId}")]
        public IActionResult GetUserData(string userId)
        {
            var profile = ProfileService.GetUserprofilebyid(userId);

            if (profile == null)
                return NotFound("User not found"); // أو 404

            return Ok(profile);
        }

    }
}
