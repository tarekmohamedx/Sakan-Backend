using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Listing
{
    public int Id { get; set; }

    public string HostId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? PricePerMonth { get; set; }

    public int? MaxGuests { get; set; }

    public string? Governorate { get; set; }

    public string? District { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsBookableAsWhole { get; set; }

    public decimal? MinBedPrice { get; set; }

    public decimal? AverageRating { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual AspNetUser Host { get; set; } = null!;

    public virtual ICollection<ListingPhoto> ListingPhotos { get; set; } = new List<ListingPhoto>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
}
