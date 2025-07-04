using Microsoft.EntityFrameworkCore;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces.Admin;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services.Admin
{
    public class AdminListingService : IAdminListingService
    {
        private readonly sakanContext _context;

        public AdminListingService(sakanContext context)
        {
            _context = context;
        }

        public async Task<ListingEditDto> GetListingByIdAsync(int id)
        {
            var listing = await _context.Listings
                .Where(l => l.Id == id && !l.IsDeleted)
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

        public async Task<object> GetAlltListingsAsync(int page, int pageSize, string? search)
        {
            var query = _context.Listings
                .Where(l => !l.IsDeleted)
                .Include(l => l.Host)
                .Select(l => new AdminHostListingDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Location = l.Governorate + ", " + l.District,
                    PricePerMonth = l.PricePerMonth ?? 0,
                    MaxGuests = l.MaxGuests ?? 0,
                    IsActive = l.IsActive,
                    PreviewImage = l.ListingPhotos.FirstOrDefault().PhotoUrl,
                    HostName = l.Host.UserName
                });
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(l =>
                    l.Title.ToLower().Contains(search) ||
                    l.HostName.ToLower().Contains(search));
            }

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

        public async Task<bool> UpdateListingWithPhotosAsync(int id, ListingEditDto updated)
        {
            var listing = await _context.Listings
                .Include(l => l.ListingPhotos)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null) return false;

            // Update basic fields
            listing.Title = updated.Title;
            listing.Description = updated.Description;
            listing.PricePerMonth = updated.PricePerMonth;
            listing.MaxGuests = updated.MaxGuests;
            listing.Governorate = updated.Governorate;
            listing.District = updated.District;
            listing.IsBookableAsWhole = updated.IsBookableAsWhole;
            listing.IsActive = updated.IsActive;

            // ✨ Update photos
            var currentUrls = listing.ListingPhotos.Select(p => p.PhotoUrl).ToList();

            // Remove photos no longer included
            var photosToRemove = listing.ListingPhotos.Where(p => !updated.PhotoUrls.Contains(p.PhotoUrl)).ToList();
            _context.RemoveRange(photosToRemove);

            // Add new photos
            var newPhotos = updated.PhotoUrls.Except(currentUrls);
            foreach (var url in newPhotos)
            {
                listing.ListingPhotos.Add(new ListingPhoto
                {
                    PhotoUrl = url,
                    ListingId = listing.Id
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteListingAsync(int id)
        {
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

            if (listing == null) return false;

            listing.IsDeleted = true;
            _context.Listings.Update(listing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetListingApprovalStatusAsync(int listingId, bool isApproved)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null || listing.IsDeleted) return false;

            listing.IsApproved = isApproved;
            //listing.IsActive = isApproved; // optionally activate only approved listings
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
