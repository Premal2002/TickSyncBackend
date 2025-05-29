using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TickSyncAPI.Dtos.Movies;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;

namespace TickSyncAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly BookingSystemContext _context;

        public MovieService(BookingSystemContext context)
        {
            _context = context;
        }
        public async Task<string> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                throw new CustomException(404, "Movie not found");
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return "Movie deleted successfully";
        }

        public async Task<Movie> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                throw new CustomException(404,"Movie not found");
            }

            return movie;
        }

        public async Task<MoviesResponse> GetMovies(MoviesFilter filter)
        {
            var query = _context.Movies.AsQueryable();

            // Filter by language
            if (filter.Languages != null && filter.Languages.Count > 0)
            {
                query = query.Where(m => filter.Languages.Contains(m.Language));
            }

            // Filter by genre
            if (filter.Genres != null && filter.Genres.Count > 0)
            {
                query = query.Where(m => filter.Genres.Any(g => m.Genre.Contains(g)));
            }

            // Get total count before pagination
            int totalMovies = await query.CountAsync();

            // Apply pagination
            var movies = await query
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .ToListAsync();

            return new MoviesResponse
            {
                Movies = movies,
                Total = totalMovies
            };
        }

        public async Task<IEnumerable<ShowVenueGroup>> GetMovieShows(int movieId)
        {
            var shows = await _context.Shows
                                      .Where(s => s.MovieId == movieId && s.ShowDate >= DateOnly.FromDateTime(DateTime.Now))
                                      .Include(s => s.Venue)
                                      .ToListAsync();

            var grouped = shows
                .GroupBy(s => s.VenueId)
                .Select(g => new ShowVenueGroup
                {
                    VenueId = g.Key,
                    Name = g.First().Venue.Name,
                    Location = g.First().Venue.Location,
                    Shows = g.ToList()
                })
                .ToList();

            return grouped;
        }

        public async Task<IEnumerable<Movie>> GetRecommendedMovies()
        {
            return await _context.Movies
                                 .Where(m => m.Rating != null && m.Rating >= 7.0)  // Only highly rated movies
                                 .OrderByDescending(m => m.ReleaseDate)
                                 .Take(10)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetRelatedMovies(int movieId)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == movieId);
            return await _context.Movies
                                 .Where(m => m.MovieId != movie.MovieId && (m.Language == movie.Language && m.Genre.Contains(movie.Genre)))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetTrendingMovies()
        {
            return await _context.Movies
                                 .OrderByDescending(m => m.Popularity)
                                 .Take(10)
                                 .ToListAsync();
        }

        public async Task<Movie> PostMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        public async Task<string> PutMovie(int id, Movie movie)
        {
            if (id != movie.MovieId)
            {
                throw new CustomException(404, "Movie not found");
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    throw new CustomException(404, "Movie not found");
                }
                else
                {
                    throw;
                }
            }

            return "Movie modified successfully";
        }

        public async Task<int> SearchMovie(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new CustomException(400, "Your query is empty");

            query = query.ToLower().Trim();
            var allMovies = await _context.Movies.ToListAsync();

            //Exact match (case-insensitive)
            var exactMatch = allMovies
                .FirstOrDefault(m => m.Title.ToLower() == query);

            if (exactMatch != null)
                return exactMatch.MovieId;

            //StartsWith match
            var startsWithMatch = allMovies
                .FirstOrDefault(m => m.Title.ToLower().StartsWith(query));

            if (startsWithMatch != null)
                return startsWithMatch.MovieId;

            //Contains match
            var containsMatch = allMovies
                .OrderBy(m => m.Title.Length) // shorter matches are more relevant
                .FirstOrDefault(m => m.Title.ToLower().Contains(query));
            if(containsMatch != null)
                return containsMatch.MovieId;


            // Find closest match using Levenshtein distance
            int minDistance = int.MaxValue;
            int bestMatchId = 0;

            foreach (var candidate in allMovies)
            {
                int distance = LevenshteinDistance(candidate.Title.ToLower(), query);
                if (distance < minDistance && distance < 5)
                {
                    minDistance = distance;
                    bestMatchId = candidate.MovieId;
                }
            }

            if(bestMatchId != 0)
                return bestMatchId;

            return 0;
        }

        private int LevenshteinDistance(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
                dp[i, 0] = i;

            for (int j = 0; j <= b.Length; j++)
                dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1,     // Deletion
                                 dp[i, j - 1] + 1),    // Insertion
                        dp[i - 1, j - 1] + cost        // Substitution
                    );
                }
            }

            return dp[a.Length, b.Length];
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
