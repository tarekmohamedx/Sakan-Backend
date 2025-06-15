using Sakan.Application.Interfaces;
using Sakan.Application.DTOs;
using Sakan.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Sakan.Infrastructure.Services
{
    public class ListingDetailsService : IListingDetailsService
    {
        private readonly sakanContext _context;

        public ListingDetailsService(sakanContext context)
        {
            _context = context;
        }

        public async Task<ListingDetailsDto> GetListingDetails(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.ListingPhotos)
                .Include(l => l.Rooms)
                .ThenInclude(r => r.RoomPhotos)
                .Include(l => l.Host)
                .FirstOrDefaultAsync(l => l.Id == id);
            


            if (listing == null) return null;

            return new ListingDetailsDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                PricePerMonth = listing.PricePerMonth ?? 0,
                Location = $"{listing.Governorate}, {listing.District}",
                Latitude = listing.Latitude ?? 0,
                Longitude = listing.Longitude ?? 0,
                Bedrooms = listing.Rooms.Count(r => r.Type.ToLower().Contains("bedroom")),
                Bathrooms = listing.Rooms.Count(r => r.Type == "bathroom"),
                Photos = listing.ListingPhotos.Select(p => p.PhotoUrl).ToList(),
                BedroomList = listing.Rooms
                .Where(r => r.Type.ToLower().Contains("bedroom"))
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    PricePerNight = r.PricePerNight,
                    Photos = r.RoomPhotos.Select(p => p.PhotoUrl).ToList()
                })
                .ToList(),
                Host = new HostInfoDto
                {
                    Name = listing.Host.UserName,
                    //    PhotoUrl = "https://www.citypng.com/public/uploads/preview/hd-man-user-illustration-icon-transparent-png-701751694974843ybexneueic.png",
                    //    Reviews = 7, // You can calculate from reviews table if available
                    //    Rating = 5.0, // Calculate or hardcode
                    //    MonthsHosting = 1, // Logic based on host.CreatedAt maybe
                    //    Location = "Pyramids Gardens, Egypt", // Update if stored
                    //    ResponseRate = 100,
                    //    ResponseTime = "within an hour",
                    //    Languages = new List<string> { "English" }
                }
            };

        }

        public async Task<List<BookedMonthDto>> GetBookedMonthsAsync(int listingId)
        {
            return await _context.Bookings
                .Where(b => b.ListingId == listingId)
                .Select(b => new BookedMonthDto
                {
                    Year = b.FromDate.Year,
                    Month = b.FromDate.Month
                })
                .Distinct()
                .ToListAsync();
        }

    }

}
