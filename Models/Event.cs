using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly EventDate { get; set; }

    public TimeOnly EventTime { get; set; }

    public int VenueId { get; set; }

    public virtual Venue Venue { get; set; } = null!;
}
