using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public int ListingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Listing Listing { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
