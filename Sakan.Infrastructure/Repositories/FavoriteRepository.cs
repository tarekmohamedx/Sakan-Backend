using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.MyContext;
using Microsoft.EntityFrameworkCore;

namespace Sakan.Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly sakanContext _context;

        public FavoriteRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<Favorite> GetAsync(string userId, int listingId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId);
        }

        public async Task<List<Listing>> GetUserFavoritesAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId && f.IsActive) // جلب المفضلات الفعالة فقط
                .Select(f => f.Listing) // اختيار الوحدات المرتبطة
                .Include(l => l.ListingPhotos) // لجلب الصور
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }
    }
}
