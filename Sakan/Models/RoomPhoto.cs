using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class RoomPhoto
{
    public int Id { get; set; }

    public int? RoomId { get; set; }

    public string? PhotoUrl { get; set; }

    public virtual Room? Room { get; set; }
}
