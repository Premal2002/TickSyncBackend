namespace TickSyncAPI.Models.Dtos
{
    public class TmdbMovie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string Original_Language { get; set; }
        public string Poster_Path { get; set; }
        public string Backdrop_Path { get; set; }
        public string Release_Date { get; set; }
        public List<int> Genre_Ids { get; set; }
        public double Vote_Average { get; set; }
        public double Popularity { get; set; }
    }

    public class TmdbResponse
    {
        public List<TmdbMovie> results { get; set; }
    }

    public class TmdbGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TmdbGenreResponse
    {
        public List<TmdbGenre> genres { get; set; }
    }

}
