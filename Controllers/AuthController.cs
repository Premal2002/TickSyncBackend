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
            try
            {
                var token = await _authService.LoginUser(user);
                return Ok(token);
            }catch(Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("assignRole")]
        public ActionResult<bool> AssignRoleToUser([FromBody] AddUserRole userRole)
        {
            try
            {
                var addedUserRole = _authService.AssignRoleToUser(userRole);
                return Ok(addedUserRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("addRole")]
        public ActionResult<Role>  AddRole([FromBody] Role role)
        {
            try
            {
                var addedRole = _authService.AddRole(role);
                return Ok(addedRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var res = await _authService.ForgotPassword(request);
                return Ok(res);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }   
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
        {
            try
            {
                var isVerifiedCode = await _authService.VerifyResetCode(request);
                return Ok(new { verified = true });
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var res = await _authService.ResetPassword(request);
                return Ok(new { message = res });
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
