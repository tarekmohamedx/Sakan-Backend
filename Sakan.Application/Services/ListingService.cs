
using Microsoft.AspNetCore.Mvc;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sakan.Application.Common;
using Sakan.Domain.Common;
using Sakan.Application.Interfaces.User;
using Sakan.Application.DTOs.User;
using Microsoft.AspNetCore.Http;

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

        public async Task<CreateListingDTO> ParseCreateListingFormAsync(IFormCollection form)
        {
            var dto = new CreateListingDTO
            {
                Title = form["Title"],
                Description = form["Description"],
                PricePerMonth = decimal.Parse(form["PricePerMonth"]),
                MaxGuests = int.Parse(form["MaxGuests"]),
                Governorate = form["Governorate"],
                District = form["District"],
                Latitude = double.Parse(form["Latitude"]),
                Longitude = double.Parse(form["Longitude"]),
                IsBookableAsWhole = bool.Parse(form["IsBookableAsWhole"]),
                AmenityIds = form["AmenityIds"].Select(int.Parse).ToList(),
                ListingPhotos = form.Files.Where(f => f.Name == "ListingPhotos").ToList(),
                Rooms = new List<CreateRoomDTO>()
            };

            int roomIndex = 0;
            while (form.ContainsKey($"Rooms[{roomIndex}].Name"))
            {
                var room = new CreateRoomDTO
                {
                    Name = form[$"Rooms[{roomIndex}].Name"],
                    Type = form[$"Rooms[{roomIndex}].Type"],
                    PricePerNight = decimal.Parse(form[$"Rooms[{roomIndex}].PricePerNight"]),
                    MaxGuests = int.Parse(form[$"Rooms[{roomIndex}].MaxGuests"]),
                    IsBookableAsWhole = bool.Parse(form[$"Rooms[{roomIndex}].IsBookableAsWhole"]),
                    RoomPhotos = form.Files.Where(f => f.Name == $"Rooms[{roomIndex}].RoomPhotos").ToList(),
                    Beds = new List<CreateBedDTO>()
                };

                int bedIndex = 0;
                while (form.ContainsKey($"Rooms[{roomIndex}].Beds[{bedIndex}].Label"))
                {
                    var bed = new CreateBedDTO
                    {
                        Label = form[$"Rooms[{roomIndex}].Beds[{bedIndex}].Label"],
                        Type = form[$"Rooms[{roomIndex}].Beds[{bedIndex}].Type"],
                        Price = decimal.Parse(form[$"Rooms[{roomIndex}].Beds[{bedIndex}].Price"]),
                        IsAvailable = bool.Parse(form[$"Rooms[{roomIndex}].Beds[{bedIndex}].IsAvailable"]),
                        BedPhotos = form.Files.Where(f => f.Name == $"Rooms[{roomIndex}].Beds[{bedIndex}].BedPhotos").ToList()
                    };

                    room.Beds.Add(bed);
                    bedIndex++;
                }

                dto.Rooms.Add(room);
                roomIndex++;
            }

            return dto;
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
                IsActive = false,
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
            if (dto.AmenityIds != null && dto.AmenityIds.Any())
            {
                listing.ListingAmenities = dto.AmenityIds.Select(id => new ListingAmenities
                {
                    AmenitiesId = id,
                    ListingsId = listing.Id
                }).ToList();
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

