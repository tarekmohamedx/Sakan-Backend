using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class SupportTicket
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? GuestName { get; set; }

    public string? GuestEmail { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public int? BookingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public string? GuestAccessToken { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<TicketReply> TicketReplies { get; set; } = new List<TicketReply>();

    public virtual AspNetUser? User { get; set; }
}
