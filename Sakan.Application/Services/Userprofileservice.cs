using Sakan.Application.DTOs;
using Sakan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class Userprofileservice : IProfileService
    {
        private readonly IProfile profile;

        public Userprofileservice(IProfile profile)
        {
            this.profile = profile;
        }

        public UserProfileDTO GetUserprofilebyid(string id)
        {
            var user = profile.Getuserbyid(id);
            var request = profile.GetRequestbyuserid(id);
            var listing = profile.Getlistbyuserid(id);

            if (user == null)
                return null; // المستخدم نفسه لازم يكون موجود

            string reservedType = null;
            string reservedName = null;

            if (request != null)
            {
                if (request.BedId != null && request.Bed != null)
                {
                    reservedType = "Bed";
                    reservedName = request.Bed.Label;
                }
                else if (request.RoomId != null && request.Room != null)
                {
                    reservedType = "Room";
                    reservedName = request.Room.Name;
                }
                else if (request.ListingId != null && request.Listing != null)
                {
                    reservedType = "Listing";
                    reservedName = request.Listing.Title;
                }
            }

            return new UserProfileDTO
            {
                UserID = user.Id,
                UserName = user.UserName,
                ReservedType = reservedType,
                ReservedName = reservedName,
                FromDate = request?.FromDate ?? DateTime.MinValue,
                ToDate = request?.ToDate ?? DateTime.MinValue,
                Hostapproved = request?.HostApproved,
                Guestapproved = request?.GuestApproved,
                Title = listing?.Title,
                Description = listing?.Description,
                Price = listing?.PricePerMonth ?? 0,
                Governorate = listing?.Governorate,
                District = listing?.District
            };
        }
    }
}
