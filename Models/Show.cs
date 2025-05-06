using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Show
{
    public int ShowId { get; set; }

    public int MovieId { get; set; }

    public int VenueId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly ShowTime { get; set; }

    public decimal RegularSeatPrice { get; set; }

    public decimal PremiumSeatPrice { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Movie Movie { get; set; } = null!;

    public virtual Venue Venue { get; set; } = null!;
}
