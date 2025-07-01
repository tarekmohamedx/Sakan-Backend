using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class TicketReply
{
    public int Id { get; set; }

    public int SupportTicketId { get; set; }

    public string AuthorId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AspNetUser Author { get; set; } = null!;

    public virtual SupportTicket SupportTicket { get; set; } = null!;
}
