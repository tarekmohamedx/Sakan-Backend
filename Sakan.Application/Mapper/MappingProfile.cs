using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Application.DTOs;
using Sakan.Domain.Models;
using AutoMapper;

namespace Sakan.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // هنا نُعرّف قاعدة التحويل من Listing إلى ListingSummaryDto
            CreateMap<Listing, ListingSummaryDto>()
                // للخصائص التي لها أسماء مختلفة أو منطق خاص
                .ForMember(
                    dest => dest.MainPhotoUrl,
                    opt => opt.MapFrom(src => src.ListingPhotos.FirstOrDefault()!.PhotoUrl)
                )
                .ForMember(
                    dest => dest.StartingPrice,
                    opt => opt.MapFrom(src => src.MinBedPrice ?? 0)
                );
            CreateMap<Amenity, AmenityDto>();

        }
    }
}
