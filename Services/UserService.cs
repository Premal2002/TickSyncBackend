using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Services
{
    public class UserService : IUserService
    {
        private readonly BookingSystemContext _context;
        private readonly IMemoryCache _memoryCache;

        public UserService(BookingSystemContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<ConfirmBookingResponse> ConfirmBooking(ConfirmBookingRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingSeats)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            if (booking == null)
                throw new CustomException(409, $"Booking not found.");

            if (booking.Status == "Confirmed")
                throw new CustomException(400, $"Booking is already confirmed.");

            // Confirm booking
            booking.Status = "Confirmed";
            _context.Bookings.Update(booking);

            var seatIds = booking.BookingSeats.Select(bs => bs.SeatId).ToList();

            // Remove seat locks from memory cache
            foreach (var seatId in seatIds)
            {
                var cacheKey = $"seat_lock:{booking.ShowId}:{seatId}";
                _memoryCache.Remove(cacheKey);
            }

            await _context.SaveChangesAsync();

            return new ConfirmBookingResponse
            {
                BookingId = booking.BookingId,
                Status = booking.Status
            };
        }

        public async Task<List<SeatAvailabilityDto>> GetSeatAvailability(int showId)
        {
            // 1. Get venue for the show
            var show = await _context.Shows
                .Where(s => s.ShowId == showId)
                .FirstOrDefaultAsync();

            if (show == null)
                throw new CustomException(409, $"Show not found.");

            // 2. Get all seats for the venue
            var allSeats = await _context.Seats
                .Where(seat => seat.VenueId == show.VenueId)
                .ToListAsync();

            // 3. Get booked seatIds for this show (only confirmed bookings)
            var bookedSeatIds = await _context.BookingSeats
                .Where(bs => bs.Booking.ShowId == showId && bs.Booking.Status == "Confirmed")
                .Select(bs => bs.SeatId)
                .ToListAsync();

            // 4. Compose response
            var seatStatusList = allSeats.Select(seat =>
            {
                string seatKey = $"seat_lock:{showId}:{seat.SeatId}";
                bool isLocked = _memoryCache.TryGetValue<SeatLockInfo>(seatKey, out var seatLock);

                string status = "Available";
                if (bookedSeatIds.Contains(seat.SeatId))
                    status = "Booked";
                else if (isLocked)
                    status = "Locked";

                return new SeatAvailabilityDto
                {
                    SeatId = seat.SeatId,
                    RowNumber = seat.RowNumber,
                    SeatNumber = seat.SeatNumber,
                    SeatType = seat.SeatType,
                    Status = status
                };
            }).ToList();

            return seatStatusList;
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
                                Price = rowGroup.First().SeatType == "Premium" ? show.PremiumSeatPrice : show.RegularSeatPrice,
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

        public async Task<InitiateBookingResponse> InitiateBooking(InitiateBookingRequest request)
        {
            var show = await _context.Shows.FirstOrDefaultAsync(s => s.ShowId == request.ShowId);
            if (show is null)
            {
                throw new CustomException(404, $"Show not found.");
            }

            var allSeats = await _context.Seats
                .Where(seat => seat.VenueId == show.VenueId)
                .Select(s => s.SeatId)
                .ToListAsync();

            foreach (var seatId in request.SeatIds)
            {
                if (!allSeats.Contains(seatId))
                {
                    throw new CustomException(404, $"Seat with id {seatId} not found in specified venue.");
                }
            }

            foreach (var seatId in request.SeatIds)
            {
                var cacheKey = $"seat_lock:{request.ShowId}:{seatId}";
                if (!_memoryCache.TryGetValue<SeatLockInfo>(cacheKey, out var lockInfo) ||
                    lockInfo == null ||
                    lockInfo.UserId != request.UserId ||
                    lockInfo.ExpiresAt <= DateTime.UtcNow)
                {
                    throw new CustomException(409, $"Seat {seatId} is not locked or lock expired.");
                }
            }

            var booking = new Booking
            {
                UserId = request.UserId,
                ShowId = request.ShowId,
                TotalAmount = request.TotalAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookingSeats = request.SeatIds.Select(seatId => new BookingSeat
            {
                BookingId = booking.BookingId,
                SeatId = seatId,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.BookingSeats.AddRange(bookingSeats);
            await _context.SaveChangesAsync();

            return new InitiateBookingResponse
                    {
                        BookingId = booking.BookingId,
                        Status = booking.Status!,
                        TotalAmount = booking.TotalAmount
                    };
        }

        public async Task<List<int>> LockSeatsInCache(SeatLockRequest request)
        {
            var show = await _context.Shows.FirstOrDefaultAsync(s => s.ShowId == request.ShowId);
            if(show is null)
            {
                throw new CustomException(404, $"Show not found.");
            }

            var allSeats = await _context.Seats
                .Where(seat => seat.VenueId == show.VenueId)
                .Select(s => s.SeatId)
                .ToListAsync();

            foreach(var seatId in request.SeatIds)
            {
                if (!allSeats.Contains(seatId))
                {
                    throw new CustomException(404, $"Seat with id {seatId} not found in specified venue.");
                }
            }

            foreach (var seatId in request.SeatIds)
            {
                string cacheKey = $"seat_lock:{request.ShowId}:{seatId}";

                if (_memoryCache.TryGetValue<SeatLockInfo>(cacheKey, out var existingLock))
                {
                    if (existingLock.ExpiresAt > DateTime.UtcNow)
                    {
                        throw new CustomException(409, $"Seat {seatId} is already locked.");
                    }
                }

                var lockInfo = new SeatLockInfo
                {
                    UserId = request.UserId,
                    LockedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10)
                };

                _memoryCache.Set(cacheKey, lockInfo, TimeSpan.FromMinutes(10));
            }
            return request.SeatIds;
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
