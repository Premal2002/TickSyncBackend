using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class Movie
{
    public int MovieId { get; set; }         // Your own primary key (or use TMDB ID here if preferred)

    public int TMDBId { get; set; }          // Store TMDB's id (e.g., 637)

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Language { get; set; }

    public int? Duration { get; set; }

    public string? Genre { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? PosterUrl { get; set; }    // From TMDB's `poster_path`

    public string? BackdropUrl { get; set; }  // From TMDB's `backdrop_path`

    public double? Rating { get; set; }       // From `vote_average`

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}

//USE[BookingSystem];
//GO

//ALTER TABLE[dbo].[Movies]
//ADD
//    [TMDBId] INT NULL,
//    [PosterUrl] NVARCHAR(500) NULL,
//    [BackdropUrl] NVARCHAR(500) NULL,
//    [Rating] FLOAT NULL;
//GO
