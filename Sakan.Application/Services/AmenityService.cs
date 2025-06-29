using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sakan.Application.DTOs;
using Sakan.Application.Interfaces;
using Sakan.Domain.Interfaces;

namespace Sakan.Application.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _amenityRepository;
        private readonly IMapper _mapper;

        public AmenityService(IAmenityRepository amenityRepository, IMapper mapper)
        {
            _amenityRepository = amenityRepository;
            _mapper = mapper;
        }

        public async Task<List<AmenityDto>> GetAllAsync()
        {
            var amenities = await _amenityRepository.GetAllAsync();
            return _mapper.Map<List<AmenityDto>>(amenities);
        }
    }
}
