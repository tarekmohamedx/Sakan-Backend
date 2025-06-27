using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Services;
using Sakan.Domain.Models;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITestService testService;
        private readonly RoleManager<IdentityRole> roleManager;
        public TestController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITestService testService)
        {
            this.userManager = userManager;

            this.roleManager = roleManager;
            this.testService = testService;
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

    }
}
