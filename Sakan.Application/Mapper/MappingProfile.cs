using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Models;
using AutoMapper;
using Sakan.Application.DTOs.User;
using Sakan.Application.DTOs;

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
            CreateMap<SupportTicket, TicketDetailsDto>()
                .ForMember(dest => dest.SubmitterName,
                       opt => opt.MapFrom(src => src.User != null ? src.User.UserName : src.GuestName))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.TicketReplies));
            CreateMap<TicketReply, TicketReplyDto>()
                .ForMember(dest => dest.AuthorName,
                       opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : "Guest"));

        }
    }
}
