using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly BookingSystemContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(BookingSystemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Role AddRole(Role role)
        {
            var addedRole = _context.Roles.Add(role);
            _context.SaveChanges();
            return addedRole.Entity;
        }

        public bool AssignRoleToUser(AddUserRole obj)
        {
            try
            {
                var addRoles = new List<UserRole>();
                var user = _context.Users.SingleOrDefault(s => s.UserId == obj.UserId);
                if (user == null)
                    throw new Exception("user is not valid");
                foreach (int role in obj.RoleIds)
                {
                    var userRole = new UserRole();
                    userRole.RoleId = role;
                    userRole.UserId = user.UserId;
                    addRoles.Add(userRole);
                }
                _context.UserRoles.AddRange(addRoles);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<TokenDto> LoginUser(UserLoginDto userDto)
        {
            if (userDto.Email != null && userDto.Password != null)
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == userDto.Email);
                if (user != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash);

                    if (isPasswordValid)
                    {
                        var claims = new List<Claim>
                        {
                        new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                        new Claim("Id",user.UserId.ToString()),
                        new Claim("Email",user.Email)
                    };

                        var userRoles = await _context.UserRoles.Where(r => r.UserId == user.UserId).ToListAsync();
                        var roleIds = userRoles.Select(r => r.RoleId).ToList();
                        var roles = await _context.Roles.Where(r => roleIds.Contains(r.RoleId)).ToListAsync();

                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: signIn);

                        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                        var tokenDto = new TokenDto()
                        {
                            JwtToken = jwtToken,
                            UserId = user.UserId
                        };

                        return tokenDto;
                    }
                    else
                    {
                        throw new Exception("Password didn't match!");
                    }
                }
                else
                {
                    throw new Exception("User is not valid!");
                }
            }
            else
            {
                throw new Exception("credentials are not valid!");
            }

        }
    }
}
