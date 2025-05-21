using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using TickSyncAPI.Dtos.Auth;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;

namespace TickSyncAPI.Services
{
    public class AuthService : IAuthService 
    {
        private readonly BookingSystemContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(BookingSystemContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
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

        public async Task<string> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordRequest.Email);

            if (user == null)
                return "If the email is registered, you'll receive a reset code.";

            var code = new Random().Next(100000, 999999).ToString();

            var resetRequest = new PasswordResetRequest
            {
                Email = forgotPasswordRequest.Email,
                SecretCode = code,
                ExpiresAt = DateTime.Now.AddMinutes(10),
                IsUsed = false
            };
            var rowsToRemove = await _context.PasswordResetRequests.Where(p => p.Email == forgotPasswordRequest.Email).ToListAsync();
            _context.PasswordResetRequests.RemoveRange(rowsToRemove);
            _context.PasswordResetRequests.Add(resetRequest);
            await _context.SaveChangesAsync();

            // TODO: Send the code via email
            string subject = "Your Password Reset Code";
            string body = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <p>Hello,</p>
                                    <p>Your password reset code is:</p>
                                    <h2 style='color:#2e6c80;'>{code}</h2>
                                    <p>This code will expire in 10 minutes.</p>
                                    <p>If you didn’t request a reset, please ignore this email.</p>
                                    <br/>
                                    <p>Thanks,<br/>TickSync Team</p>
                                </body>
                            </html>";
            await _emailService.SendEmail(forgotPasswordRequest.Email, subject, body);

            return "If the email is registered, you'll receive a reset code.";
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
                        new Claim("id",user.UserId.ToString()),
                        new Claim("email",user.Email),
                        new Claim("name",user.FullName)

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
                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: signIn);

                        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                        var tokenDto = new TokenDto()
                        {
                            JwtToken = jwtToken,
                            UserId = user.UserId,
                            FullName = user.FullName,
                            Email = user.Email
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

        public async Task<string> ResetPassword(ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new CustomException(400, "Passwords do not match.");

            var resetRecord = await _context.PasswordResetRequests
                .Where(r => r.Email == request.Email && r.SecretCode == request.SecretCode && !r.IsUsed && r.ExpiresAt > DateTime.Now)
                .FirstOrDefaultAsync();

            if (resetRecord == null)
                throw new CustomException(400, "Invalid or expired code.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new CustomException(400, "User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            resetRecord.IsUsed = true;

            await _context.SaveChangesAsync();
            return "Password Reset Successful";
        }
        public async Task<bool> VerifyResetCode(VerifyResetCodeRequest verifyResetCodeRequest)
        {
            var record = await _context.PasswordResetRequests
                .Where(r => r.Email == verifyResetCodeRequest.Email && !r.IsUsed && r.ExpiresAt > DateTime.Now)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();

            if (record == null || record.SecretCode != verifyResetCodeRequest.SecretCode)
                throw new CustomException(400, "Invalid or expired reset code.");

            return true;
        }
    }
}
