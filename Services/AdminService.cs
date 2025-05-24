using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TickSyncAPI.Dtos.Admin;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;

namespace TickSyncAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly BookingSystemContext _context;

        public AdminService(BookingSystemContext context)
        {
            _context = context;
        }
        public async Task<List<object>> GetAllConfirmedBookings()
        {
            var bookings = await _context.Bookings.Include(b => b.User).Include(b => b.Show).ThenInclude(s => s.Movie).Select(b => new
            {
                BookingId = b.BookingId,
                User = b.User.FullName,
                Movie = b.Show.Movie.Title,
                ShowDate = b.Show.ShowDate,
                ShowTime = b.Show.ShowTime,
                BookedDate = b.CreatedAt,
                TotalAmount = b.TotalAmount,
                Status = b.Status
            }).Where(b => b.Status == "Confirmed").ToListAsync();
            return bookings.Cast<object>().ToList();
        }

        public async Task<AllDataCountDto> GetAllDataCounts()
        {
           var usersCount = await _context.Users.CountAsync();
           var moviesCount = await _context.Movies.CountAsync();
           var venuesCount = await _context.Venues.CountAsync();
           var showsCount = await _context.Shows.CountAsync();
           var seatsCount = await _context.Seats.CountAsync();

            var allDataCount = new AllDataCountDto
            {
                Users = usersCount,
                Movies = moviesCount,
                Venues = venuesCount,
                Shows = showsCount,
                Seats = seatsCount
            };
            return allDataCount;
        }

        public async Task<List<object>> GetAllPayments()
        {
            var payments = await _context.Payments.Where(p => p.Status == "Success").Select(p => new
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                UserId = p.UserId,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod,
                PaidAt = p.PaidAt
            }).ToListAsync();
            return payments.Cast<object>().ToList();
        }

        public async Task<EntityDataDto<object>> GetEntityData(string entity)
        {
            switch (entity.ToLower())
            {
                case "users":
                    var users = await _context.Users.Select(u => new
                    {
                        Name = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone,
                        RegistrationDate = u.CreatedAt
                    }).ToListAsync();
                    return new EntityDataDto<object>
                    {
                        Title = "users",
                        EntityData = users.Cast<object>().ToList()
                    };

                case "shows":
                    var shows = await _context.Shows.Include(s => s.Movie).Select(s => new
                    {
                        Movie = s.Movie.Title,
                        ShowDate = s.ShowDate,
                        ShowTime = s.ShowTime,
                        RegularSeatPrice = s.RegularSeatPrice,
                        PremiumSeatPrice = s.PremiumSeatPrice
                    }).ToListAsync();
                    return new EntityDataDto<object>
                    {
                        Title = "shows",
                        EntityData = shows.Cast<object>().ToList()
                    };

                case "movies":
                    var movies = await _context.Movies.Select(m => new
                    {
                        Title = m.Title,
                        Language = m.Language,
                        Genre = m.Genre,
                        ReleaseDate = m.ReleaseDate,
                        Rating = m.Rating
                    }).ToListAsync();
                    return new EntityDataDto<object>
                    {
                        Title = "movies",
                        EntityData = movies.Cast<object>().ToList()
                    };

                case "venues":
                    var venues = await _context.Venues.Select(v => new
                    {
                        Name = v.Name,
                        Location = v.Location,
                        TotalSeats = v.TotalSeats
                    }).ToListAsync();
                    return new EntityDataDto<object>
                    {
                        Title = "venues",
                        EntityData = venues.Cast<object>().ToList()
                    };

                case "seats":
                    var seats = await _context.Seats.Include(s => s.Venue).Select(s => new
                    {
                        SeatNumber = s.SeatNumber,
                        RowNumber = s.RowNumber,
                        SeatType = s.SeatType,
                        VenueName = s.Venue.Name
                    }).ToListAsync();
                    return new EntityDataDto<object>
                    {
                        Title = "seats",
                        EntityData = seats.Cast<object>().ToList()
                    };

                case "confirmedbookings":
                    var confirmBookings = await GetAllConfirmedBookings();
                    return new EntityDataDto<object>
                    {
                        Title = "confirmBookings",
                        EntityData = confirmBookings
                    };
                
                case "payments":
                    var payments = await GetAllPayments();
                    return new EntityDataDto<object>
                    {
                        Title = "Payments",
                        EntityData = payments.Cast<object>().ToList()
                    };

                default:
                    throw new CustomException(404,$"Unknown entity type '{entity}'");
            }
        }
    }
}
