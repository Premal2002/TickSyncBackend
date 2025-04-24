using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly BookingSystemContext _context;
        public AuthService(BookingSystemContext context)
        {
            _context = context;
        }
        public async Task<string> LoginUser(UserLoginDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null) return "Invalid credentials";

            // Verify the password using bcrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return "Invalid credentials";

            return "Login successful";
        }
    }
}
