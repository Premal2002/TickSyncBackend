using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Dtos;
using TickSyncAPI.Dtos.Booking;
using Microsoft.AspNetCore.Mvc;

namespace TickSyncAPI.Interfaces
{
    public interface IBookingService
    {
        public Task<SeatLockRequest> LockSeatsInCache(SeatLockRequest request);
        public Task<ShowSeatLayoutDto> GetLatestSeatsLayout(int showId);
        public Task<InitiateBookingResponse> InitiateBooking(InitiateBookingRequest request);
        //public Task<ConfirmBookingResponse> ConfirmBooking(ConfirmBookingRequest request);
        public Task<bool> CancelBooking(CancelBookingRequest request);
        public Task<List<UserBookingsResponse>> GetUserBookings(int userId);
        public Task<CreateOrderResponse> CreateRazorpayOrder(CreateOrderRequest request);
        public Task<string> PaymentCallback(PaymentCallbackRequest request);
    }
}
