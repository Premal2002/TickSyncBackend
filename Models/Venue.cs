using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TickSyncAPI.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public int TotalSeats { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    [JsonIgnore]
    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
