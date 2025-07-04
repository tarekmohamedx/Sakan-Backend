using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Amenity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? IconUrl { get; set; }

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
