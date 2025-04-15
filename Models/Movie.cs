﻿using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Language { get; set; }

    public int? Duration { get; set; }

    public string? Genre { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
