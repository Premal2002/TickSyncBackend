using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Dtos;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Dtos.Booking;
using Microsoft.AspNetCore.Authorization;

namespace TickSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("lockSeats")]
        public async Task<IActionResult> LockSeats([FromBody] SeatLockRequest request)
        {
           var result = await _bookingService.LockSeatsInCache(request);
           return Ok(result);
        }

        [HttpGet("getLatestSeatsLayout/{showId}")]
        public async Task<IActionResult> GetLatestSeatsLayout(int showId)
        {
            var result = await _bookingService.GetLatestSeatsLayout(showId);
            return Ok(result);
        }

        [HttpPost("initiateBooking")]
        public async Task<ActionResult<InitiateBookingResponse>> InitiateBooking([FromBody] InitiateBookingRequest request)
        {
            var result = await _bookingService.InitiateBooking(request);
            return Ok(result);
        }

        [HttpPost("cancelBooking")]
        public async Task<ActionResult> CancelBooking([FromBody] CancelBookingRequest request)
        {
            var result = await _bookingService.CancelBooking(request);
            return Ok(result);
        }
        
        [HttpPost("createRazorpayOrder")]
        public async Task<ActionResult<CreateOrderResponse>> CreateRazorpayOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _bookingService.CreateRazorpayOrder(request);
            return Ok(result);
        }

        [HttpPost("paymentCallback")]
        public async Task<IActionResult> PaymentCallback(PaymentCallbackRequest request)
        {
            var result = await _bookingService.PaymentCallback(request);
            return Ok(new { message = result });
        }

        [HttpGet("getBookingHistory/{userId}")]
        public async Task<IActionResult> GetBookingHistory(int userId)
        {
            var result = await _bookingService.GetUserBookingHistory(userId);
            return Ok(result);
        }

        [HttpGet("getBookingData/{bookingId}")]
        public async Task<ActionResult> GetUserBooking(int bookingId)
        {
            var result = await _bookingService.GetUserBookingData(bookingId);
            return Ok(result);
        }
    }
}
