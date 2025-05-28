using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Dtos.Movies;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;

namespace TickSyncAPI.Controllers
{
    //Adding comment to test build pipeline and main branch policy
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="user,admin")]
    public class MoviesController : ControllerBase
    {
        private readonly ITmbdService _tmdbService;
        private readonly IMovieService _movieService;

        public MoviesController(ITmbdService tmdbService, IMovieService movieService)
        {
            _tmdbService = tmdbService;
            _movieService = movieService;
        }

        // GET: api/Movies
        [HttpPost("getMovies")]
        public async Task<ActionResult<object>> GetMovies([FromBody] MoviesFilter filter)
        {
            var res = await _movieService.GetMovies(filter);
            return Ok(res);
        }

        // GET: api/Movies/Trending - based on popularity
        [HttpGet("Trending")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetTrendingMovies()
        {
           var res = await _movieService.GetTrendingMovies();
           return Ok(res);
        }

        // GET: api/Movies/Trending - based on rating and release date
        [HttpGet("Recommended")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetRecommendedMovies()
        {
           var res = await _movieService.GetRecommendedMovies();
           return Ok(res);
        }

        // GET: api/Movies/getRelatedMovies - get related similar movies based on a movie
        [HttpPost("getRelatedMovies/{movieId}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetRelatedMovies(int movieId)
        {
            var res = await _movieService.GetRelatedMovies(movieId);
            return Ok(res);
        }

        // GET: api/Movies/getMovieShows/${movieId} - To get all the upcoming shows of a movie 
        [HttpGet("getMovieShows/{movieId}")]
        public async Task<ActionResult<IEnumerable<ShowVenueGroup>>> GetMovieShows(int movieId)
        {
           var res = await _movieService.GetMovieShows(movieId);
           return Ok(res);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
           var res = await _movieService.GetMovie(id);
           return Ok(res);
        }

        [HttpGet("TMDB/Movies")]
        public async Task<ActionResult> GetMovieTMDB()
        {
           var result = await _tmdbService.GetMovieTMDB();
           return Ok(result);
        }

        [HttpPost("sync-tmdb-movies")]
        public async Task<IActionResult> SyncTMDBMovies()
        {
           var result = await _tmdbService.SyncTMDBMovies();
           return Ok(result);
        }

        //To upadae movies table to add popularity column and values from TMDB 
        [HttpPost("update-tmdb-popularity")]
        public async Task<IActionResult> UpdateTMDBPopularity()
        {
            var result = await _tmdbService.UpdateTMDBPopularity();
            return Ok(result);
        }

        //to sync all the laguages in language table from tmdb language endpoint
        [HttpPost("sync-tmdb-languages")]
        public async Task<IActionResult> SyncTMDBLanguages()
        {
            var result = await _tmdbService.SyncTMDBLanguages();
            return Ok(result);
        }


        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            var res = await _movieService.PutMovie(id,movie);
            return Ok(res);
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            var res = await _movieService.PostMovie(movie);
            return Ok(res);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var res = await _movieService.DeleteMovie(id);
            return Ok(res);
        }
        
        // search movie
        [HttpGet("searchMovie/{query}")]
        public async Task<IActionResult> SearchMovie(string query)
        {
            var res = await _movieService.SearchMovie(query);
            return Ok(res);
        }
    }
}
