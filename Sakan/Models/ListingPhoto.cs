using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class ListingPhoto
{
    public int Id { get; set; }

    public int? ListingId { get; set; }

    public string? PhotoUrl { get; set; }

    public virtual Listing? Listing { get; set; }
}
