using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.Dtos.Movies;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Models;

namespace TickSyncAPI.Interfaces
{
    public interface IMovieService
    {
        public Task<MoviesResponse> GetMovies(MoviesFilter filter);
        public Task<Movie> GetMovie(int id);
        public Task<Movie> PostMovie(Movie movie);
        public Task<string> PutMovie(int id, Movie movie);
        public Task<string> DeleteMovie(int id);
        public Task<IEnumerable<Movie>> GetTrendingMovies();
        public Task<IEnumerable<Movie>> GetRecommendedMovies();
        public Task<IEnumerable<Movie>> GetRelatedMovies(int movieId);
        public Task<IEnumerable<ShowVenueGroup>> GetMovieShows(int movieId);
        public Task<int> SearchMovie(string query);

    }
}
