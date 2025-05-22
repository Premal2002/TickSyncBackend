using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TickSyncAPI.Models;

public partial class BookingSeat
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int SeatId { get; set; }

    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual Booking Booking { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
