using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;


namespace Sakan.Infrastructure.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly sakanContext _context;

        public AmenityRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task<List<Amenity>> GetAllAsync()
        {
            // AsNoTracking لتحسين الأداء لأنها عملية قراءة فقط
            return await _context.Amenities.AsNoTracking().ToListAsync();
        }

        Task<List<Domain.Models.Amenity>> IAmenityRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
