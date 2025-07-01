using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class BookingRequest
{
    public int Id { get; set; }

    public string? GuestId { get; set; }

    public int? ListingId { get; set; }

    public int? RoomId { get; set; }

    public int? BedId { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public bool? HostApproved { get; set; }

    public bool? GuestApproved { get; set; }

    public bool IsActive { get; set; }

    public virtual Bed? Bed { get; set; }

    public virtual AspNetUser? Guest { get; set; }

    public virtual Listing? Listing { get; set; }

    public virtual Room? Room { get; set; }
}
