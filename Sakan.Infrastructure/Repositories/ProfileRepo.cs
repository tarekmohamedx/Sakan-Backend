using Microsoft.AspNetCore.Identity;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Repositories
{
   
    public class ProfileRepo : IProfile
    {
        private readonly sakanContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileRepo(sakanContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public ApplicationUser Getuserbyid(string id)
        {
            var user = userManager.Users.FirstOrDefault(x => x.Id == id);
            return user; 
        }

        Listing IProfile.Getlistbyuserid(string id)
        {
            var list = context.Listings.FirstOrDefault(s => s.HostId == id);
            return list; 
        }

        BookingRequest IProfile.GetRequestbyuserid(string id)
        {
            var request = context.BookingRequests.FirstOrDefault(s => s.GuestId == id);
            return request; 
        }
    }
}
