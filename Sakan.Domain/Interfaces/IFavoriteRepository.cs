using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Models;

namespace Sakan.Domain.Interfaces
{
    public interface IFavoriteRepository
    {
        // البحث عن سجل مفضلة معين
        Task<Favorite> GetAsync(string userId, int listingId);

        // جلب كل الوحدات المفضلة للمستخدم
        Task<List<Listing>> GetUserFavoritesAsync(string userId);

        // إضافة سجل مفضلة جديد
        Task AddAsync(Favorite favorite);

        // لا نحتاج لـ Update صريح لأن EF Core يتتبع التغييرات
        // public void Update(Favorite favorite); 
    }
}
