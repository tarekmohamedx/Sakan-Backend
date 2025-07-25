﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Common;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Sakan.Infrastructure.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly sakanContext _context;

        public ListingRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<(List<Listing> Items, int TotalCount)> GetFilteredListingsAsync(ListingFilterParameters filterParams)
        {
            var query = _context.Listings
                .Include(l => l.ListingPhotos)
                .Include(l => l.Rooms).ThenInclude(r => r.Beds).ThenInclude(b => b.Bookings)
                .Where(l => l.IsApproved == true && l.IsActive == true && l.IsDeleted==false)
                .AsNoTracking(); // AsNoTracking لتحسين الأداء لأننا لن نعدل على البيانات

            // --- تطبيق الفلاتر ---
            if (filterParams.MinPrice.HasValue)
                query = query.Where(l => l.MinBedPrice >= filterParams.MinPrice.Value);

            if (filterParams.MaxPrice.HasValue)
                query = query.Where(l => l.MinBedPrice <= filterParams.MaxPrice.Value);

            if (filterParams.CheckInDate.HasValue && filterParams.CheckOutDate.HasValue)
                query = query.Where(l => l.Rooms.SelectMany(r => r.Beds).Any(b => !b.Bookings.Any(bk =>
                    bk.FromDate < filterParams.CheckOutDate.Value && bk.ToDate > filterParams.CheckInDate.Value)));

            if (filterParams.NorthEastLat.HasValue &&
                filterParams.NorthEastLng.HasValue &&
                filterParams.SouthWestLat.HasValue &&
                filterParams.SouthWestLng.HasValue)
            {
                query = query.Where(l =>
                    l.Latitude >= filterParams.SouthWestLat.Value &&
                    l.Latitude <= filterParams.NorthEastLat.Value &&
                    l.Longitude >= filterParams.SouthWestLng.Value &&
                    l.Longitude <= filterParams.NorthEastLng.Value);
            }

            if (!string.IsNullOrEmpty(filterParams.Governorate))
                query = query.Where(l => l.Governorate == filterParams.Governorate);

            if (filterParams.AmenityIds != null && filterParams.AmenityIds.Any())
                foreach (var amenityId in filterParams.AmenityIds)
                    query = query.Where(l => l.ListingAmenities.Any(a => a.AmenitiesId == amenityId));

            if (filterParams.MinRating.HasValue)
                query = query.Where(l => l.AverageRating >= filterParams.MinRating.Value);

            if (!string.IsNullOrEmpty(filterParams.RoomType))
            {
                // هذا الشرط يبحث عن الوحدات التي تحتوي على الأقل على غرفة واحدة من النوع المطلوب
                query = query.Where(l => l.Rooms.Any(r => r.Type == filterParams.RoomType));
            }

            // --- حساب العدد الإجمالي ---
            var totalCount = await query.CountAsync();

            var sortedQuery = ApplySorting(query, filterParams.SortBy);

            // --- تطبيق الترتيب والترقيم ثم عمل الـ Projection ---
            var items = await sortedQuery
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private IQueryable<Listing> ApplySorting(IQueryable<Listing> query, string? sortBy)
        {
            return sortBy?.ToLowerInvariant() switch
            {
                "price_asc" => query.OrderBy(l => l.MinBedPrice),
                "price_desc" => query.OrderByDescending(l => l.MinBedPrice),
                "rating_desc" => query.OrderByDescending(l => l.AverageRating),
                _ => query.OrderByDescending(l => l.CreatedAt) // الترتيب الافتراضي
            };
        }
        public async Task<(List<Listing> Items, int TotalCount)> GetAllListingsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Listings
                .Include(l => l.ListingPhotos)
                .Where(l => l.IsApproved == true && l.IsActive == true && l.IsDeleted == false)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Listing>> GetHighestRatedListingsAsync(int count)
        {
            return await _context.Listings
                .Include(l => l.ListingPhotos)
                .Where(l => l.IsApproved == true && l.IsActive == true && l.IsDeleted == false)
                .OrderByDescending(l => l.AverageRating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Listing>> GetNewestListingsAsync(int count)
        {
            return await _context.Listings
                .Include(l => l.ListingPhotos)
                .Where(l => l.IsApproved == true && l.IsActive == true && l.IsDeleted == false)
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Listing>> GetMostAffordableListingsAsync(int count)
        {
            return await _context.Listings
                .Include(l => l.ListingPhotos)
                .Where(l => l.IsApproved == true && l.IsActive == true && l.MinBedPrice > 0 && l.IsDeleted == false)
                .OrderBy(l => l.MinBedPrice)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(List<Listing> Items, int TotalCount)> GetAllNotApprovedListingAsync(int pageNumber, int pageSize)
        {
            var query = _context.Listings
                .Include(l => l.ListingPhotos)
                .Include(l => l.Host)
                .Where(l => l.IsApproved == false)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<bool> ApproveExistingListing(int listingId)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null) throw new Exception("Listing not found");

            listing.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectExistingListing(int listingId)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null) throw new Exception("Listing not found");

            listing.IsApproved = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
