using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Interfaces
{
    public interface IProfile
    {
        BookingRequest GetRequestbyuserid(string id); 

        Listing Getlistbyuserid(string id); 
        ApplicationUser Getuserbyid(string id); 

    }
}
