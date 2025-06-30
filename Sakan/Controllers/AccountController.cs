using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Sakan.Application.DTOs;
using Sakan.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sakan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserName = registerDTO.Username,
                    Email = registerDTO.Email,
                    PhoneNumber = registerDTO.PhoneNumber,
                };

                // First try to create the user
                IdentityResult result = await userManager.CreateAsync(user, registerDTO.Password);

                if (result.Succeeded)
                {
                    try
                    {
                        // Then try to add the role
                        var roleResult = await userManager.AddToRoleAsync(user, "Customer");

                        if (!roleResult.Succeeded)
                        {
                            // If role assignment fails, delete the user
                            await userManager.DeleteAsync(user);

                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return BadRequest(ModelState);
                        }

                        // Continue with token generation if everything succeeded
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(ClaimTypes.Email, user.Email));
                        claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

                        var roles = await userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var signkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));

                        //var key = config["jwt:key"];
                        //var signkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
                        //Console.WriteLine("🔐 LOGIN: jwt:key = " + key);


                        SigningCredentials signingCredentials = new SigningCredentials(signkey, SecurityAlgorithms.HmacSha256);

                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: config["jwt:issuer"],
                            audience: config["jwt:audience"],
                            expires: DateTime.UtcNow.AddDays(1),
                            claims: claims,
                            signingCredentials: signingCredentials
                        );

                        return Ok(new
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                        });
                    }
                    catch (Exception ex)
                    {
                        // If any exception occurs during role assignment or token generation,
                        // attempt to delete the user
                        await userManager.DeleteAsync(user);
                        return StatusCode(500, "An error occurred during registration");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            var errors = ModelState.Values.SelectMany(s => s.Errors).Select(s => s.ErrorMessage).ToList();
            return BadRequest(errors);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                return BadRequest(new { Message = "Validation failed", Errors = modelErrors });
            }

            var user = await userManager.FindByEmailAsync(loginDTO.email);

            if (user == null)
            {
                return Unauthorized(new { Message = "User with this email does not exist." });
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!passwordValid)
            {
                return Unauthorized(new { Message = "Incorrect password." });
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Prepare the signing key
            var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));

            //var key = config["jwt:key"];
            //var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            //System.Diagnostics.Debug.WriteLine("🔐 LOGIN: jwt:key = " + key);


            var signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: config["jwt:issuer"],
                audience: config["jwt:audience"], // ✅ fix typo here: was "jst"
                expires: DateTime.UtcNow.AddDays(1),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            });
        }

    }

}
