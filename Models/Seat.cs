using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int VenueId { get; set; }

    public string RowNumber { get; set; } = null!;

    public string SeatNumber { get; set; } = null!;

    public string SeatType { get; set; } = null!;

    public string? Status { get; set; }

    public virtual Venue Venue { get; set; } = null!;
}
