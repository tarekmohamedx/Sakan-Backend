using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public string? ReviewerId { get; set; }

    public string? ReviewedUserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int BookingId { get; set; }

    public bool IsActive { get; set; }

    public int? ListingId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Listing? Listing { get; set; }

    public virtual AspNetUser? ReviewedUser { get; set; }

    public virtual AspNetUser? Reviewer { get; set; }
}
