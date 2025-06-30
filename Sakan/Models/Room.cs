using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Room
{
    public int Id { get; set; }

    public int? ListingId { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public decimal? PricePerNight { get; set; }

    public int? MaxGuests { get; set; }

    public bool? IsBookableAsWhole { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Bed> Beds { get; set; } = new List<Bed>();

    public virtual ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Listing? Listing { get; set; }

    public virtual ICollection<RoomPhoto> RoomPhotos { get; set; } = new List<RoomPhoto>();
}
