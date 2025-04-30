using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

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
        public bool AssignRoleToUser([FromBody] AddUserRole userRole)
        {
            var addedUserRole = _authService.AssignRoleToUser(userRole);
            return addedUserRole;
        }

        [HttpPost("addRole")]
        public ActionResult<Role>  AddRole([FromBody] Role role)
        {
            var addedRole = _authService.AddRole(role);
            return Ok(addedRole);
        }
    }
}
