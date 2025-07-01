using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public string? StripePaymentIntentId { get; set; }

    public decimal? Amount { get; set; }

    public string? Currency { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Method { get; set; }

    public virtual Booking? Booking { get; set; }
}
