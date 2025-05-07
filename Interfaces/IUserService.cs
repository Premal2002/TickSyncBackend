using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUser(UserRegisterDto user);
        public Task<ShowSeatLayoutDto> GetShowSeatLayout(int showId);
    }
}
