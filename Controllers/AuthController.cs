using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.Dtos.Auth;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;

namespace TickSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto user)
        {
            var token = await _authService.LoginUser(user);
            return Ok(token);
        }

        [HttpPost("assignRole")]
        public ActionResult<bool> AssignRoleToUser([FromBody] AddUserRole userRole)
        {
            var addedUserRole = _authService.AssignRoleToUser(userRole);
            return Ok(addedUserRole);
        }

        [HttpPost("addRole")]
        public ActionResult<Role>  AddRole([FromBody] Role role)
        {
            var addedRole = _authService.AddRole(role);
            return Ok(addedRole);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var res = await _authService.ForgotPassword(request);
            return Ok(res);   
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
        {
            var isVerifiedCode = await _authService.VerifyResetCode(request);
            return Ok(new { verified = true });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var res = await _authService.ResetPassword(request);
            return Ok(new { message = res });
        }
    }
}
