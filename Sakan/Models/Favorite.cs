using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Favorite
{
    public string UserId { get; set; } = null!;

    public int ListingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Listing Listing { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
