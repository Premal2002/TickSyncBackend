using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Services
{
    public class UserService : IUserService
    {
        private readonly BookingSystemContext _context;

        public UserService(BookingSystemContext context)
        {
            _context = context;
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
