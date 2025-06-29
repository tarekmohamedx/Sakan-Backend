
﻿using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sakan.Application.Common;
using Sakan.Domain.Common;

namespace Sakan.Application.Services
{
    public class ListingService : IListingService
    {

        private readonly IListRepository _listingRepository;
        private readonly IListingRepository _listingRepository2;
        private readonly IImageKitService _imageKitService;
        private readonly IMapper _mapper;

        public ListingService(IImageKitService imageKitService, IListRepository listingRepository, IMapper mapper, IListingRepository listingRepository2)
        {
            _imageKitService = imageKitService;
            _listingRepository = listingRepository;
            _mapper = mapper;
            _listingRepository2 = listingRepository2;
        }

        public async Task CreateListingAsync(CreateListingDTO dto, string hostId)
        {
            var listing = new Listing
            {
                Title = dto.Title,
                Description = dto.Description,
                PricePerMonth = dto.PricePerMonth,
                MaxGuests = dto.MaxGuests,
                Governorate = dto.Governorate,
                District = dto.District,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsBookableAsWhole = dto.IsBookableAsWhole,
                CreatedAt = DateTime.UtcNow,
                IsApproved = false,
                IsActive = true,
                HostId = hostId,
            };

            // 1. Upload listing photos
            foreach (var photo in dto.ListingPhotos)
            {
                var url = await _imageKitService.UploadImageAsync(photo, "/listings");
                listing.ListingPhotos.Add(new ListingPhoto { PhotoUrl = url });
            }

            // 2. Add Rooms
            foreach (var roomDto in dto.Rooms)
            {
                var room = new Room
                {
                    Name = roomDto.Name,
                    Type = roomDto.Type,
                    PricePerNight = roomDto.PricePerNight,
                    MaxGuests = roomDto.MaxGuests,
                    IsBookableAsWhole = roomDto.IsBookableAsWhole
                };

                // 2.a Upload Room Photos
                foreach (var roomPhoto in roomDto.RoomPhotos)
                {
                    var roomPhotoUrl = await _imageKitService.UploadImageAsync(roomPhoto, "/rooms");
                    room.RoomPhotos.Add(new RoomPhoto { PhotoUrl = roomPhotoUrl });
                }

                // 2.b Add Beds
                foreach (var bedDto in roomDto.Beds)
                {
                    var bed = new Bed
                    {
                        Label = bedDto.Label,
                        Type = bedDto.Type,
                        Price = bedDto.Price,
                        IsAvailable = bedDto.IsAvailable
                    };

                    // 2.c Upload Bed Photos
                    foreach (var bedPhoto in bedDto.BedPhotos)
                    {
                        var bedPhotoUrl = await _imageKitService.UploadImageAsync(bedPhoto, "/beds");
                        bed.BedPhotos.Add(new BedPhoto { PhotoUrl = bedPhotoUrl });
                    }

                    room.Beds.Add(bed);
                }

                listing.Rooms.Add(room);
            }

            // Save to DB
            await _listingRepository.Addlistasync(listing);
            await _listingRepository.Savechangeasync();
        }

        public async Task<PagedResult<ListingSummaryDto>> GetFilteredListingsAsync(ListingFilterParameters filterParams)
        {
            var (listings, totalCount) = await _listingRepository2.GetFilteredListingsAsync(filterParams);

            // استخدام AutoMapper للتحويل
            var dtos = _mapper.Map<List<ListingSummaryDto>>(listings);

            return new PagedResult<ListingSummaryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            };
        }

        public async Task<PagedResult<ListingSummaryDto>> GetAllListingsAsync(int pageNumber, int pageSize)
        {
            var (listings, totalCount) = await _listingRepository2.GetAllListingsAsync(pageNumber, pageSize);

            // استخدام AutoMapper للتحويل
            var dtos = _mapper.Map<List<ListingSummaryDto>>(listings);

            return new PagedResult<ListingSummaryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<ListingSummaryDto>> GetHighestRatedListingsAsync(int count)
        {
            var listings = await _listingRepository2.GetHighestRatedListingsAsync(count);
            // استخدام AutoMapper مباشرة
            return _mapper.Map<List<ListingSummaryDto>>(listings);
        }

        public async Task<List<ListingSummaryDto>> GetNewestListingsAsync(int count)
        {
            var listings = await _listingRepository2.GetNewestListingsAsync(count);
            return _mapper.Map<List<ListingSummaryDto>>(listings);
        }

        public async Task<List<ListingSummaryDto>> GetMostAffordableListingsAsync(int count)
        {
            var listings = await _listingRepository2.GetMostAffordableListingsAsync(count);
            return _mapper.Map<List<ListingSummaryDto>>(listings);
        }




    }














}

