namespace TickSyncAPI.Dtos.Movie
{
    public class MoviesFilter
    {
        public List<string> Languages { get; set; }
        public List<string> Genres { get; set; }
        public int Page { get; set; } = 1; // Default to page 1
        public int Limit { get; set; } = 8; // Default to 8 movies per page
    }
}
