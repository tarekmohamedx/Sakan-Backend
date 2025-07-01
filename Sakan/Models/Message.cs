using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public string? Content { get; set; }

    public DateTime? Timestamp { get; set; }

    public int? ChatId { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual AspNetUser? Receiver { get; set; }

    public virtual AspNetUser? Sender { get; set; }
}
