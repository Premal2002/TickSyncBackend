using TickSyncAPI.Models;

namespace TickSyncAPI.Dtos.Movies
{
    public class MoviesResponse
    {
        public List<Movie> Movies { get; set; }
        public int Total { get; set; } 
    }
}
