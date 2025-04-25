using TickSyncAPI.Models;

namespace TickSyncAPI.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUser(User user);
    }
}
