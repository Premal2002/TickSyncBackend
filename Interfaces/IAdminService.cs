using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Models;

namespace TickSyncAPI.Interfaces
{
    public interface IAdminService
    {
        public Task<List<Booking>> GetAllConfirmedBookings();
        public Task<List<Show>> GetAllShows();
        public Task<List<Payment>> GetAllPayments();

    }
}
