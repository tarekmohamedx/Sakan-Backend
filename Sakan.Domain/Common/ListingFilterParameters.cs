using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Domain.Common
{
    public class ListingFilterParameters
    {
        // Filters
        public double? NorthEastLat { get; set; }
        public double? NorthEastLng { get; set; }
        public double? SouthWestLat { get; set; }
        public double? SouthWestLng { get; set; }
        public string? Governorate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public decimal? MinRating { get; set; }
        public List<int>? AmenityIds { get; set; }
        public string? RoomType { get; set; }

        // Sorting
        public string? SortBy { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
