using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using Sakan.Domain.Interfaces;
using Sakan.Domain.IUnitOfWork;
using Sakan.Domain.Models;

namespace Sakan.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteService(IFavoriteRepository favoriteRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, int listingId)
        {
            var existingFavorite = await _favoriteRepository.GetAsync(userId, listingId);

            if (existingFavorite != null)
            {
                // السجل موجود، سنقوم بعكس حالته
                existingFavorite.IsActive = !existingFavorite.IsActive;
            }
            else
            {
                // السجل غير موجود، سنقوم بإنشاء واحد جديد
                var newFavorite = new Favorite
                {
                    UserId = userId,
                    ListingId = listingId,
                    IsActive = true, // عند إضافته لأول مرة يكون فعالاً
                    CreatedAt = DateTime.UtcNow
                };
                await _favoriteRepository.AddAsync(newFavorite);
            }

            await _unitOfWork.SaveChangesAsync();
            // نرجع الحالة الجديدة
            return existingFavorite?.IsActive ?? true;
        }

        public async Task<List<ListingSummaryDto>> GetUserFavoritesAsync(string userId)
        {
            var favoriteListings = await _favoriteRepository.GetUserFavoritesAsync(userId);
            return _mapper.Map<List<ListingSummaryDto>>(favoriteListings);
        }
    }
}
