using Microsoft.EntityFrameworkCore;
using TickSyncAPI.HelperClasses;
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
        public async Task<ShowSeatLayoutDto> GetShowSeatLayout(int showId)
        {
                var show = await _context.Shows
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.ShowId == showId);

                if (show == null)
                    throw new CustomException(404, "Show not found.");

                var seatTypeGroups = await _context.Seats
                    .AsNoTracking()
                    .Where(seat => seat.VenueId == show.VenueId)
                    .GroupBy(seat => seat.SeatType)
                    .Select(seatTypeGroup => new SeatTypeGroupDto
                    {
                        SeatType = seatTypeGroup.Key,
                        Price = seatTypeGroup.Key == "Premium" ? show.PremiumSeatPrice : show.RegularSeatPrice,
                        Rows = seatTypeGroup
                            .GroupBy(seat => seat.RowNumber)
                            .Select(rowGroup => new RowDto
                            {
                                RowNumber = rowGroup.Key,
                                SeatType = seatTypeGroup.Key,
                                Price = rowGroup.Key == "Premium" ? show.PremiumSeatPrice : show.RegularSeatPrice,
                                Seats = rowGroup
                                    .OrderBy(seat => seat.SeatNumber)
                                    .Select(seat => new SeatDto
                                    {
                                        SeatId = seat.SeatId,
                                        RowNumber = seat.RowNumber,
                                        SeatNumber = seat.SeatNumber,
                                        SeatType = seat.SeatType,
                                        Status = seat.Status,
                                        Price = seat.SeatType == "Premium" ? show.PremiumSeatPrice : show.RegularSeatPrice
                                    })
                                    .ToList()
                            })
                            .OrderBy(row => row.RowNumber)
                            .ToList()
                    })
                    .ToListAsync();

                return new ShowSeatLayoutDto
                {
                    ShowId = show.ShowId,
                    ShowDate = show.ShowDate,
                    ShowTime = show.ShowTime,
                    VenueId = show.VenueId,
                    seatTypeGroup = seatTypeGroups
                };
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
