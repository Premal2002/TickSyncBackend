using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Dtos;

namespace TickSyncAPI.Interfaces
{
    public interface IBookingService
    {
        public Task<List<int>> LockSeatsInCache(SeatLockRequest request);
        public Task<ShowSeatLayoutDto> GetLatestSeatsLayout(int showId);
        public Task<InitiateBookingResponse> InitiateBooking(InitiateBookingRequest request);
        public Task<ConfirmBookingResponse> ConfirmBooking(ConfirmBookingRequest request);

    }
}
