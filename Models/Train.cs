using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Train
{
    public int TrainId { get; set; }

    public string TrainName { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public DateOnly DepartureDate { get; set; }

    public TimeOnly DepartureTime { get; set; }

    public TimeOnly ArrivalTime { get; set; }
}
