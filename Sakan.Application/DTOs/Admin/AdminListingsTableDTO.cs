using Sakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.Admin
{
    public class AdminListingsTableDTO
    {
        public int Id { get; set; }

        public string HostId { get; set; } = string.Empty;

        public string HostName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal? PricePerMonth { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string PhotoUrl { get; set; }
    }
}
