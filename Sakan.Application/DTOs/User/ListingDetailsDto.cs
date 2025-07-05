using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class ListingDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerMonth { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public List<string> Photos { get; set; }
        public List<RoomDto> BedroomList { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public HostInfoDto Host { get; set; }
    }

    public class HostInfoDto
    {
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public int Reviews { get; set; }
        public double Rating { get; set; }
        public int MonthsHosting { get; set; }
        public string Location { get; set; }
        public int ResponseRate { get; set; }
        public string ResponseTime { get; set; }
        public List<string> Languages { get; set; }
    }


}
