using System.Net.Http.Headers;
using System.Text.Json;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.HelperClasses;
using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Dtos.Movies;

namespace TickSyncAPI.Services
{
    public class TmdbService : ITmbdService
    {
        private readonly BookingSystemContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TmdbService(BookingSystemContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<JsonElement> GetMovieTMDB()
        {
            var bearerToken = _configuration.GetValue<string>("BearerTokenTMDB");
            var endpoint = $"https://api.themoviedb.org/3/movie/now_playing?language=en-US&page=1";
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new CustomException((int)response.StatusCode, $"TMDB API error: {errorMsg}");
            }

            var json = await response.Content.ReadAsStringAsync();

            // Optionally: Deserialize into your DTO
            var result = JsonSerializer.Deserialize<JsonElement>(json); // or your custom DTO

            return result;
        }

        public async Task<string> SyncTMDBLanguages()
        {
            var bearerToken = _configuration.GetValue<string>("BearerTokenTMDB");
            var endpoint = "https://api.themoviedb.org/3/configuration/languages";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new CustomException((int)response.StatusCode, $"TMDB API error: {errorMsg}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var tmdbLanguages = JsonSerializer.Deserialize<List<TmdbLanguageDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            foreach (var tmdbLanguage in tmdbLanguages)
            {
                // Check if already exists
                var exists = await _context.Languages
                    .AnyAsync(l => l.IsoCode == tmdbLanguage.Iso_639_1);

                if (!exists)
                {
                    var language = new Language
                    {
                        IsoCode = tmdbLanguage.Iso_639_1,
                        EnglishName = tmdbLanguage.English_Name,
                        NativeName = tmdbLanguage.Name
                    };

                    _context.Languages.Add(language);
                }
            }

            await _context.SaveChangesAsync();

            return "Languages synced successfully.";
        }

        public async Task<string> SyncTMDBMovies()
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

            return "TMDB movies synced successfully.";
        }

        public async Task<string> UpdateTMDBPopularity()
        {
            var bearerToken = _configuration.GetValue<string>("BearerTokenTMDB");

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
                    // Find movie by TMDBId
                    var existingMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Tmdbid == tmdbMovie.Id);

                    if (existingMovie != null)
                    {
                        existingMovie.Popularity = tmdbMovie.Popularity;  // ✅ Only update Popularity field
                    }
                }

                await _context.SaveChangesAsync();
            }

            return "Movie Popularities updated successfully.";
        }
    }
}
