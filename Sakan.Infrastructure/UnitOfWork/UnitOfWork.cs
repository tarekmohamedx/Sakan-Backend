using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.IUnitOfWork;
using Sakan.Infrastructure.Models;

namespace Sakan.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly sakanContext _context;
        public UnitOfWork(sakanContext context) => _context = context;
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
        public void Dispose() => _context.Dispose();
    }
}
