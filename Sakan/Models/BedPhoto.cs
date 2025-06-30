using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class BedPhoto
{
    public int Id { get; set; }

    public int? BedId { get; set; }

    public string? PhotoUrl { get; set; }

    public virtual Bed? Bed { get; set; }
}
