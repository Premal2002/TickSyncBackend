using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Dtos.Auth;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;

namespace TickSyncAPI.Services
{
    public class UserService : IUserService
    {
        private readonly BookingSystemContext _context;

        public UserService(BookingSystemContext context)
        {
            _context = context;
        }

        public async Task<string> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new CustomException(404, "User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return "User deleted successfully";
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                throw new CustomException(404,"User not found");
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<string> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                throw new CustomException(404, "User not found");
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
                    throw new CustomException(404, "User not found");
                }
                else
                {
                    throw;
                }
            }

            return "User modified successfully";
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public async Task<User> RegisterUser(UserRegisterDto userDto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);
            var user = new User()
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                Phone = userDto.Phone,
                PasswordHash = passwordHash
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
