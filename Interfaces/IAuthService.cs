using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Interfaces
{
    public interface IAuthService
    {
        public Task<string> LoginUser(UserLoginDto user);
    }
}
