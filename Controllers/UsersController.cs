using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Models;
using TickSyncAPI.Interfaces;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Dtos;
using TickSyncAPI.Dtos.Auth;
using TickSyncAPI.Dtos.Seat;

namespace TickSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            
            return await _userService.GetUser(id);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            var res = await _userService.PutUser(id,user);
            return Ok(res);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserRegisterDto userDto)
        {
            try
            {
                var createdUser = await _userService.RegisterUser(userDto);
                return Ok(createdUser);
            }
            catch(DbUpdateException ex)
            {
                var message = ex.InnerException?.Message;

                if (message?.Contains("UQ_Users_Email") == true)
                    return BadRequest("Email already exists.");
                return BadRequest(ex.Message );
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message );
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var res = await _userService.DeleteUser(id);
            return Ok(res);
        }

    }
}
