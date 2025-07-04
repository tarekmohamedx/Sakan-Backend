using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string GuestId { get; set; } = null!;

    public int? ListingId { get; set; }

    public int? RoomId { get; set; }

    public int? BedId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public decimal? Price { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Bed? Bed { get; set; }

    public virtual AspNetUser Guest { get; set; } = null!;

    public virtual Listing? Listing { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Room? Room { get; set; }

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
