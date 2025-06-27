using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sakan.Application.DTOs;
using Sakan.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using System.Net;

namespace Sakan.Controllers
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
                        SigningCredentials signingCredentials = new SigningCredentials(signkey, SecurityAlgorithms.HmacSha256);

                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: config["jwt:issuer"],
                            audience: config["jwt:audiance"],
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
                return Unauthorized(new { Message = "this email does not exist." });
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

        //    [HttpGet("sakanak")]
        //    public async Task<IActionResult> ExternalLoginCallback()
        //    {
        //        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //        if (!result.Succeeded)
        //            return BadRequest("External login failed.");

        //        var externalUser = result.Principal;
        //        var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;

        //        var user = await userManager.FindByEmailAsync(email);

        //        if (user == null)
        //        {
        //            user = new ApplicationUser
        //            {
        //                UserName = email,
        //                Email = email
        //            };
        //            await userManager.CreateAsync(user);
        //            await userManager.AddToRoleAsync(user, "Customer");
        //        }

        //        // Now generate JWT token
        //        var claims = new List<Claim>
        //{
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim(ClaimTypes.NameIdentifier, user.Id),
        //    new Claim(ClaimTypes.Name, user.UserName),
        //    new Claim(ClaimTypes.Email, user.Email)
        //};

        //        var roles = await userManager.GetRolesAsync(user);
        //        foreach (var role in roles)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, role));
        //        }

        //        var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
        //        var token = new JwtSecurityToken(
        //            issuer: config["jwt:issuer"],
        //            audience: config["jwt:audience"],
        //            expires: DateTime.UtcNow.AddDays(1),
        //            claims: claims,
        //            signingCredentials: new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256)
        //        );

        //        return Ok(new
        //        {
        //            Token = new JwtSecurityTokenHandler().WriteToken(token),
        //            Expiration = token.ValidTo
        //        });
        //    }

        //[HttpPost("sakanak")]
        //public async Task<IActionResult> GoogleLogin([FromBody] GoogleloginDTO model)
        //{
        //    try
        //    {
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, new GoogleJsonWebSignature.ValidationSettings()
        //        {
        //            Audience = new[] { config["Google:ClientId"] }
        //        });

        //        // Here you can find/create the user in your DB

        //        var claims = new[]
        //        {
        //        new Claim(ClaimTypes.NameIdentifier, payload.Subject),
        //        new Claim(ClaimTypes.Name, payload.Name),
        //        new Claim(ClaimTypes.Email, payload.Email),
        //    };

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: config["Jwt:Issuer"],
        //            audience: config["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddDays(1),
        //            signingCredentials: creds);

        //        return Ok(new
        //        {
        //            token = new JwtSecurityTokenHandler().WriteToken(token),
        //            expiration = token.ValidTo
        //        });
        //    }
        //    catch
        //    {
        //        return BadRequest("Invalid Google token");
        //    }
        //}

        // AccountController.cs
        



        [HttpGet("externallogin/google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/api/Account/google-callback",
                // Force account selection
                Items = { { "prompt", "select_account" } }
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null) return BadRequest("Error loading external login info.");

            try
            {
                var token = await GenerateJwtToken(info);

                // Secure redirect with token
                return Redirect($"{config["Frontend:Url"]}/auth/google/callback?token={WebUtility.UrlEncode(token)}");
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Google authentication failed");
                return Redirect($"{config["Frontend:URL"]}/login?error=google_auth_failed");
            }
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



    }



}


