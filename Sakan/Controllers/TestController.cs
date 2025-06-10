using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sakan.Domain.Models;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public TestController(UserManager<ApplicationUser> userManager,  RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
           
            this.roleManager = roleManager;
        }


        [HttpGet]
        public  IActionResult getallroles()
        {
            var roles = roleManager.Roles.ToList(); 
            return Ok(roles);
        }

    }
}
