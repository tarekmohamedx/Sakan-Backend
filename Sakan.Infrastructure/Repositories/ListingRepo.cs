using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Repositories
{
    public class ListingRepo : IListRepository
    {
        private readonly sakanContext sakanContext;

        public ListingRepo(sakanContext sakanContext)
        {
            this.sakanContext = sakanContext;
        }

        public async Task Addlistasync(Listing listing)
        {
         await sakanContext.Listings.AddAsync(listing); 
           
        }

        public async Task Savechangeasync()
        {
            await sakanContext.SaveChangesAsync(); 
        }
    }
}
