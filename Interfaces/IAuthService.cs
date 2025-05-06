using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Interfaces
{
    public interface IAuthService
    {
        public Task<TokenDto> LoginUser(UserLoginDto user);
        public Role AddRole(Role role);
        public bool AssignRoleToUser(AddUserRole obj);
        public Task<string> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        public Task<bool> VerifyResetCode(VerifyResetCodeRequest verifyResetCodeRequest);
        public Task<string> ResetPassword(ResetPasswordRequest request);
    }
}
