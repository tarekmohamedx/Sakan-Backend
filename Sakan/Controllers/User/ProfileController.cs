using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Sakan.Application.DTOs;
using Sakan.Application.Services;
using Sakan.Domain.Models;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService profileService;
        private readonly UserManager<ApplicationUser>  userManager;
      //  private readonly ImageKitService imageKitService;



        public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager)
        {
            this.profileService = profileService;
            this.userManager = userManager;
           // this.imageKitService = imageKitService;
        }

        //api/Profile/id
        [HttpGet("{id}")]
        public IActionResult GetUserprofilebyid(string id)
        {
            var profileuser = profileService.GetUserprofilebyid(id); 
            if (profileuser == null)
                return NotFound(); 


            return Ok(profileuser); 
        }


        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateUserProfileDTO dto)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Profile updated successfully" });
        }
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteProfile(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            if (user.IsDeleted)
                return BadRequest("User is already marked as deleted");

            try
            {
                user.IsDeleted = true;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(new { Message = "User has been deleted"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }


        //[HttpPost("upload-photo/{id}")]
        //public async Task<IActionResult> UploadPhoto(string id, IFormFile photo)
        //{
        //    var user = await userManager.FindByIdAsync(id);
        //    if (user == null)
        //        return NotFound("User not found");

        //    if (photo == null || photo.Length == 0)
        //        return BadRequest("No photo uploaded");

        //    try
        //    {
        //        // Upload to ImageKit and get the URL
        //        string imageUrl = await imageKitService.UploadImageAsync(photo, "profile-pictures");

        //        // Save the image URL in user's profile
        //        user.ProfilePictureUrl = imageUrl;
        //        var result = await userManager.UpdateAsync(user);

        //        if (!result.Succeeded)
        //            return BadRequest(result.Errors);

        //        return Ok(new { Message = "Photo uploaded successfully", ImageUrl = imageUrl });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "Image upload failed", Error = ex.Message });
        //    }
        //}



    }
}
