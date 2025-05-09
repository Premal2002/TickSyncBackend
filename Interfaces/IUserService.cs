using TickSyncAPI.Models;
using TickSyncAPI.Models.Dtos;

namespace TickSyncAPI.Interfaces
{
    public interface IUserService
    {
        public Task<User> RegisterUser(UserRegisterDto user);
        public Task<ShowSeatLayoutDto> GetShowSeatLayout(int showId);
        public Task<List<int>> LockSeatsInCache(SeatLockRequest request);
        public Task<List<SeatAvailabilityDto>> GetSeatAvailability(int showId);
        public Task<InitiateBookingResponse> InitiateBooking(InitiateBookingRequest request);
        public Task<ConfirmBookingResponse> ConfirmBooking(ConfirmBookingRequest request);

    }
}
