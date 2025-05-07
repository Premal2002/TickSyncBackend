using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Models;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models.Dtos;
using TickSyncAPI.HelperClasses;

namespace TickSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BookingSystemContext _context;
        private readonly IUserService _userService;

        public UsersController(BookingSystemContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        [HttpGet("layout/{showId}")]
        public async Task<ActionResult<ShowSeatLayoutDto>> GetSeatLayout(int showId)
        {
            try
            {
                var result = await _userService.GetShowSeatLayout(showId);
                return result;
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
