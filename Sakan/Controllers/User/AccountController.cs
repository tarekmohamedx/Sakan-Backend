using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Sakan.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using System.Net;
using Microsoft.AspNetCore.Authentication.Google;
using MailKit;
using Sakan.Application.Services;

using Sakan.Application.DTOs.User;
using Sakan.Application.DTOs;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
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
                return Unauthorized(new { Message = "This email does not exist." });
            }

            // ✅ Check if the user is soft-deleted
            if (user.IsDeleted)
            {
                return Unauthorized(new { Message = "Account is deactivated." });
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

            var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
            var signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["jwt:issuer"],
                audience: config["jwt:audience"],
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

        [HttpPost("google-auth")]
        public async Task<IActionResult> ExternalLogin([FromBody] GoogleloginDTO dto)
        {
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(dto.Token);

            var user = await userManager.FindByEmailAsync(validPayload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = validPayload.Email,
                    UserName = validPayload.Email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user);
            }

            var token = await GenerateJwtToken(user);

            return Ok(new { token });
        }


        //[HttpGet("google-callback")]
        //public async Task<IActionResult> GoogleCallback()
        //{
        //    var info = await signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //        return Redirect($"{config["Frontend:Url"]}/login?error=google-failed");

        //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //    var user = await userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        user = new ApplicationUser
        //        {
        //            UserName = email,
        //            Email = email,
        //        };

        //        await userManager.CreateAsync(user);
        //        await userManager.AddToRoleAsync(user, "Customer");
        //    }

        //    var token = await GenerateJwtToken(user);
        //    return Redirect($"{config["Frontend:Url"]}/auth/google/callback?token={WebUtility.UrlEncode(token)}");
        //}

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName)
    };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["jwt:issuer"],
                audience: config["jwt:audience"],
                expires: DateTime.UtcNow.AddDays(1),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        private async Task<string> GenerateJwtToken(ExternalLoginInfo info)
        {
            // Get or create user from Google info
            var user = await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));

            if (user == null)
            {
                // Auto-provision user if not exists (optional)
                user = new ApplicationUser
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };
                await userManager.CreateAsync(user);
            }

            // Create claims (matches your existing Login method)
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // Add roles if needed
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Generate token (identical to your Login method)
            var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
            var signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["jwt:issuer"],
                audience: config["jwt:audience"],
                expires: DateTime.UtcNow.AddDays(1),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model, [FromServices] EmailService mailService)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                return BadRequest(new { message = "Invalid user or email not confirmed" });

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{config["Frontend:Url"]}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            var subject = "Reset Your Password";
            var body = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>{resetLink}</a>";

            await mailService.SendEmailAsync(user.Email, subject, body);

            return Ok(new { Message = "Password reset link has been sent to your email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { message = "Password reset failed", errors = result.Errors });

            return Ok(new { message = "Password has been reset successfully" });
        }





    }



}


