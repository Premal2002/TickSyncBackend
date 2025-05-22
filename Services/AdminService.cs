using Microsoft.EntityFrameworkCore;
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
        public async Task<List<Booking>> GetAllConfirmedBookings()
        {
            var bookings = await _context.Bookings.Where(b => b.Status == "Confirmed").ToListAsync();
            return bookings;
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            var payments = await _context.Payments.Where(p => p.Status == "Success").Include(p => p.Booking).ThenInclude(b => b.BookingSeats).ToListAsync();
            return payments;
        }

        public async Task<List<Show>> GetAllShows()
        {
            var shows = await _context.Shows.ToListAsync();
            return shows;
        }
    }
}
