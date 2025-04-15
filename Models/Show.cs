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

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<SeatLock> SeatLocks { get; set; } = new List<SeatLock>();

    public virtual Venue Venue { get; set; } = null!;
}
