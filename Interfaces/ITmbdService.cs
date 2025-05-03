using System.Text.Json;

namespace TickSyncAPI.Interfaces
{
    public interface ITmbdService
    {
        public Task<JsonElement> GetMovieTMDB();
        public Task<string> SyncTMDBMovies();
        public Task<string> UpdateTMDBPopularity();
        public Task<string> SyncTMDBLanguages();
    }
}
