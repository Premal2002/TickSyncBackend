using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Models;

namespace TickSyncAPI.Controllers
{
    //Adding comment to test build pipeline and main branch policy
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="user,admin")]
    public class MoviesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly BookingSystemContext _context;
        private readonly IConfiguration _configuration;

        public MoviesController(BookingSystemContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
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

        [HttpGet("/TMDB/Movies")]
        public async Task<ActionResult> GetMovieTMDB()
        {
            var bearerToken = _configuration.GetValue<string>("BearerTokenTMDB");
            var endpoint = $"https://api.themoviedb.org/3/movie/now_playing?language=en-US&page=1";
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error fetching TMDB data");
            }

            var json = await response.Content.ReadAsStringAsync();

            // Optionally: Deserialize into your DTO
            var result = JsonSerializer.Deserialize<JsonElement>(json); // or your custom DTO

            return Ok(result);
        }

        [HttpPost("sync-tmdb-movies")]
        public async Task<IActionResult> SyncTMDBMovies()
        {
            var bearerToken = _configuration.GetValue<string>("BearerTokenTMDB");
            var genreEndpoint = "https://api.themoviedb.org/3/genre/movie/list?language=en";
            var genreRequest = new HttpRequestMessage(HttpMethod.Get, genreEndpoint);
            genreRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var genreResponse = await _httpClient.SendAsync(genreRequest);
            var genreJson = await genreResponse.Content.ReadAsStringAsync();
            var genreData = JsonSerializer.Deserialize<TmdbGenreResponse>(genreJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var genreMap = genreData.genres.ToDictionary(g => g.Id, g => g.Name);

            for (int page = 1; page <= 5; page++)
            {
                var endpoint = $"https://api.themoviedb.org/3/movie/now_playing?language=en-US&page={page}";
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    continue; // skip this page if it fails
                }

                var json = await response.Content.ReadAsStringAsync();
                var tmdbData = JsonSerializer.Deserialize<TmdbResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var tmdbMovie in tmdbData.results)
                {
                    // Convert genre_ids to comma-separated genre names
                    var genreNames = tmdbMovie.Genre_Ids
                        .Where(id => genreMap.ContainsKey(id))
                        .Select(id => genreMap[id])
                        .ToList();

                    var genreString = string.Join(", ", genreNames);

                    var movie = new Movie
                    {
                        Tmdbid = tmdbMovie.Id,
                        Title = tmdbMovie.Title,
                        Description = tmdbMovie.Overview,
                        Language = tmdbMovie.Original_Language,
                        Genre = genreString,
                        PosterUrl = $"https://image.tmdb.org/t/p/w500{tmdbMovie.Poster_Path}",
                        BackdropUrl = $"https://image.tmdb.org/t/p/w780{tmdbMovie.Backdrop_Path}",
                        ReleaseDate = DateOnly.TryParse(tmdbMovie.Release_Date, out var date) ? date : null,
                        Rating = tmdbMovie.Vote_Average
                    };

                    // Check for duplicate TMDBId
                    var exists = await _context.Movies.AnyAsync(m => m.Tmdbid == movie.Tmdbid);
                    if (!exists)
                    {
                        _context.Movies.Add(movie);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok("TMDB movies synced successfully.");
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
