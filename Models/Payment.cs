using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
