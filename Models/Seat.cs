using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int VenueId { get; set; }

    public string? RowNumber { get; set; }

    public string? SeatNumber { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<SeatLock> SeatLocks { get; set; } = new List<SeatLock>();

    public virtual Venue Venue { get; set; } = null!;
}
