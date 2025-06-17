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
    public class TestRepo : ITestRepo
    {
        private readonly sakanContext context;

        public TestRepo(sakanContext context)
        {
            this.context = context;
        }

        public List<Test> getall()
        {
            var test = context.Tests.ToList();
            return test;
        }
    }
}
