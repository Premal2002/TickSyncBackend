using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class SeatLock
{
    public int LockId { get; set; }

    public int SeatId { get; set; }

    public int ReferenceId { get; set; }

    public int? ShowId { get; set; }

    public string? BookingType { get; set; }

    public int UserId { get; set; }

    public DateTime? LockedAt { get; set; }

    public DateTime ExpiryTime { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual Show? Show { get; set; }

    public virtual User User { get; set; } = null!;
}
