using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public string? BookingType { get; set; }

    public string ReferenceId { get; set; } = null!;

    public int? ShowId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Show? Show { get; set; }

    public virtual User User { get; set; } = null!;
}
