using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Interfaces
{
    public interface IAuthService
    {
        public Task<TokenDto> LoginUser(UserLoginDto user);
        Role AddRole(Role role);
        bool AssignRoleToUser(AddUserRole obj);
    }
}
