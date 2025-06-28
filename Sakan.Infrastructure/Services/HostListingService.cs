using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;

namespace Sakan.Infrastructure.Services
{
    public class HostListingService : IHostListingService
    {
        private readonly sakanContext _context;

        public HostListingService(sakanContext context)
        {
            _context = context;
        }

        public async Task<object> GetHostListingsPagedAsync(string hostId, int page, int pageSize)
        {
            var query = _context.Listings
                .Where(l => l.HostId == hostId)
                .Select(l => new HostListingDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Location = l.Governorate + ", " + l.District,
                    PricePerMonth = l.PricePerMonth ?? 0,
                    MaxGuests = l.MaxGuests ?? 0,
                    PreviewImage = l.ListingPhotos.FirstOrDefault().PhotoUrl
                });

            var total = await query.CountAsync();
            var listings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new
            {
                totalCount = total,
                page,
                pageSize,
                listings
            };
        }


        public async Task<ListingEditDto> GetListingByIdAsync(int id, string hostId)
        {
            var listing = await _context.Listings
                .Where(l => l.Id == id && l.HostId == hostId)
                .Select(l => new ListingEditDto
                {
                    Title = l.Title,
                    Description = l.Description,
                    PricePerMonth = l.PricePerMonth,
                    MaxGuests = l.MaxGuests,
                    Governorate = l.Governorate,
                    District = l.District,
                    IsBookableAsWhole = l.IsBookableAsWhole,
                    IsActive = l.IsActive,
                    PhotoUrls = l.ListingPhotos.Select(p => p.PhotoUrl).ToList()
                })
                .FirstOrDefaultAsync();

            return listing;
        }

        public async Task<bool> UpdateListingWithPhotosAsync(int id, string hostId, ListingEditDto updated)
        {
            var listing = await _context.Listings
                //.Include(l => l.ListingPhotos)
                .FirstOrDefaultAsync(l => l.Id == id && l.HostId == hostId);

            if (listing == null) return false;

            // Update listing fields
            listing.Title = updated.Title;
            listing.Description = updated.Description;
            listing.PricePerMonth = updated.PricePerMonth;
            listing.MaxGuests = updated.MaxGuests;
            listing.Governorate = updated.Governorate;
            listing.District = updated.District;
            listing.IsBookableAsWhole = updated.IsBookableAsWhole;
            listing.IsActive = updated.IsActive;

            //// Replace photos
            //listing.ListingPhotos.Clear();
            //foreach (var photoUrl in updated.PhotoUrls)
            //{
            //    listing.ListingPhotos.Add(new ListingPhoto
            //    {
            //        PhotoUrl = photoUrl,
            //        ListingId = listing.Id
            //    });
            //}

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteListingAsync(int id, string hostId)
        {
            var listing = await _context.Listings.FirstOrDefaultAsync(l => l.Id == id && l.HostId == hostId);
            if (listing == null) return false;

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
