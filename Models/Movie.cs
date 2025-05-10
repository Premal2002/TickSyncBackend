using System;
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

    public int? Tmdbid { get; set; }

    public string? PosterUrl { get; set; }

    public string? BackdropUrl { get; set; }

    public double? Rating { get; set; }

    public double? Popularity { get; set; }

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
