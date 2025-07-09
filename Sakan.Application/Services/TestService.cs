using Sakan.Application.DTOs.User;
using Sakan.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepo testRepo;

        public TestService(ITestRepo testRepo)
        {
            this.testRepo = testRepo;
        }

        public List<TestDTO> testsnames()
        {
            return testRepo.getall().Select(t => new TestDTO { Name = t.Name }).ToList();
        }
    }
}
