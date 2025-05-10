using TickSyncAPI.Dtos.Auth;
using TickSyncAPI.Models;

namespace TickSyncAPI.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> GetUser(int id);
        public Task<string> PutUser(int id, User user);
        public Task<string> DeleteUser(int id);
        public Task<User> RegisterUser(UserRegisterDto user);

    }
}
