using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Controllers
{
    //Adding comment to test build pipeline and main branch policy
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="user,admin")]
    public class MoviesController : ControllerBase
    {
        private readonly BookingSystemContext _context;
        private readonly ITmbdService _tmdbService;

        public MoviesController(BookingSystemContext context, ITmbdService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // GET: api/Movies
        [HttpPost("getMovies")]
        public async Task<ActionResult<List<Movie>>> GetMovies(MoviesFilter filter = null)
        {
            List<Movie> filteredMovies = await _context.Movies.ToListAsync();
            if (filter != null)
            {
                if (filter.Languages != null && filter.Languages.Count > 0)
                {
                    filteredMovies = filteredMovies.Where(m => filter.Languages.Contains(m.Language)).ToList();
                }
                    
                if (filter.Genres != null && filter.Genres.Count > 0)
                {
                    //filteredMovies = filteredMovies.Where(m => !string.IsNullOrEmpty(m.Genre) &&
                    //                          m.Genre.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                    //                           .Any(g => filter.Genres.Contains(g))).ToList();
                    //Above one is slower approach 
                    filteredMovies = filteredMovies.Where(m => filter.Genres.Any(g => m.Genre.Contains(g))).ToList();
                }
            }
            return filteredMovies;
        }
        
        // GET: api/Movies/Trending - based on popularity
        [HttpGet("Trending")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetTrendingMovies()
        {
            return await _context.Movies
                                 .OrderByDescending(m => m.Popularity)
                                 .Take(10)
                                 .ToListAsync();
        }

        // GET: api/Movies/Trending - based on rating and release date
        [HttpGet("Recommended")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetRecommendedMovies()
        {
            return await _context.Movies
                                 .Where(m => m.Rating != null && m.Rating >= 7.0)  // Only highly rated movies
                                 .OrderByDescending(m => m.ReleaseDate)
                                 .Take(10)
                                 .ToListAsync();
        }

        // GET: api/Movies/getMovieShows/${movieId} - To get all the upcoming shows of a movie 
        [HttpGet("getMovieShows/${movieId}")]
        public async Task<ActionResult<IEnumerable<IGrouping<int, Show>>>> GetMovieShows(int movieId)
        {
            return await _context.Shows
                                 .Where(s => s.MovieId == movieId && s.ShowDate >= DateOnly.FromDateTime(DateTime.Now))
                                 .GroupBy(s => s.VenueId)
                                 .ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        [HttpGet("TMDB/Movies")]
        public async Task<ActionResult> GetMovieTMDB()
        {
            try
            {
                var result = await _tmdbService.GetMovieTMDB();
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("sync-tmdb-movies")]
        public async Task<IActionResult> SyncTMDBMovies()
        {
            try
            {
                var result = await _tmdbService.SyncTMDBMovies();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        //To upadae movies table to add popularity column and values from TMDB 
        [HttpPost("update-tmdb-popularity")]
        public async Task<IActionResult> UpdateTMDBPopularity()
        {
            try
            {
                var result = await _tmdbService.UpdateTMDBPopularity();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        //to sync all the laguages in language table from tmdb language endpoint
        [HttpPost("sync-tmdb-languages")]
        public async Task<IActionResult> SyncTMDBLanguages()
        {
            try
            {
                var result = await _tmdbService.SyncTMDBLanguages();
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }


        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.MovieId)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.MovieId }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
