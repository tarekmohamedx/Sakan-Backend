using Sakan.Application.DTOs;
using Sakan.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.Interfaces.User;
using Sakan.Application.DTOs.User;

namespace Sakan.Infrastructure.Services.User
{
    public class ListingDetailsService : IListingDetailsService
    {
        private readonly sakanContext _context;

        public ListingDetailsService(sakanContext context)
        {
            _context = context;
        }

        //public async Task<ListingDetailsDto> GetListingDetails(int id)
        //{
        //    var listing = await _context.Listings
        //        .Include(l => l.ListingPhotos)
        //        .Include(l => l.Rooms)
        //        .ThenInclude(r => r.RoomPhotos)
        //        .Include(l => l.Host)
        //        .FirstOrDefaultAsync(l => l.Id == id);

        //    if (listing == null) return null;
        //    var bedroomTypes = new[] { "single", "double", "studio", "shared" };
        //    return new ListingDetailsDto
        //    {
        //        Id = listing.Id,
        //        Title = listing.Title,
        //        Description = listing.Description,
        //        PricePerMonth = listing.PricePerMonth ?? 0,
        //        Location = $"{listing.Governorate}, {listing.District}",
        //        Latitude = listing.Latitude ?? 0,
        //        Longitude = listing.Longitude ?? 0,
        //        Bedrooms = listing.Rooms.Count(r => r.Type != null && bedroomTypes.Contains(r.Type.ToLower())),
        //        Bathrooms = listing.Rooms.Count(r => r.Type == "bathroom"),
        //        Photos = listing.ListingPhotos.Select(p => p.PhotoUrl).ToList(),
        //        BedroomList = listing.Rooms
        //            .Where(r => r.Type != null && bedroomTypes.Contains(r.Type.ToLower()))
        //            .Select(r => new RoomDto
        //            {
        //                Id = r.Id,
        //                Name = r.Name,
        //                PricePerNight = r.PricePerNight,
        //                Photos = r.RoomPhotos.Select(p => p.PhotoUrl).ToList()
        //            })
        //            .ToList(),
        //        Host = new HostInfoDto
        //        {
        //            Name = listing.Host.UserName,

        //        }
        //    };

        //}


        public async Task<ListingDetailsDto> GetListingDetails(int id)
        {
            var bedroomTypes = new[] { "single", "double", "studio", "shared" };

            var listing = await _context.Listings
                .Include(l => l.ListingPhotos)
                .Include(l => l.Rooms)
                    .ThenInclude(r => r.RoomPhotos)
                .Include(l => l.Rooms)
                    .ThenInclude(r => r.Beds) // ✅ Include Beds
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
                Bedrooms = listing.Rooms.Count(r => r.Type != null && bedroomTypes.Contains(r.Type.ToLower())),
                Bathrooms = listing.Rooms.Count(r => r.Type == "bathroom"),
                Photos = listing.ListingPhotos.Select(p => p.PhotoUrl).ToList(),

                BedroomList = listing.Rooms
                    .Where(r => r.Type != null && bedroomTypes.Contains(r.Type.ToLower()))
                    .Select(r => new RoomDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        PricePerNight = r.PricePerNight,
                        Photos = r.RoomPhotos.Select(p => p.PhotoUrl).ToList(),
                        Beds = r.Beds.Select(b => new BedDto
                        {
                            Id = b.Id,
                            Label = b.Label,
                            Type = b.Type,
                            Price = b.Price,
                            IsAvailable = b.IsAvailable, // ✅ Ensure this is sent
                            BedPhotos = b.BedPhotos.Select(bp => bp.PhotoUrl).ToList()
                        }).ToList()
                    }).ToList(),

                Host = new HostInfoDto
                {
                    Name = listing.Host.UserName
                }
            };
        }


        public async Task<List<BookedMonthDto>> GetBookedMonthsAsync(int listingId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.ListingId == listingId && b.IsActive)
                .Select(b => new { b.FromDate, b.ToDate })
                .ToListAsync();

            var bookedMonths = new HashSet<BookedMonthDto>();

            foreach (var booking in bookings)
            {
                var start = new DateTime(booking.FromDate.Year, booking.FromDate.Month, 1);
                var end = new DateTime(booking.ToDate.Year, booking.ToDate.Month, 1);

                for (var date = start; date <= end; date = date.AddMonths(1))
                {
                    bookedMonths.Add(new BookedMonthDto
                    {
                        Year = date.Year,
                        Month = date.Month
                    });
                }
            }

            return bookedMonths.ToList();
        }



    }

}
