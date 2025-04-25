using Microsoft.EntityFrameworkCore;
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
        public async Task<User> RegisterUser(User user)
        {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
        }
    }
}
