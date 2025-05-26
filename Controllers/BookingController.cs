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
            try
            {
                var result = await _bookingService.LockSeatsInCache(request);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("getLatestSeatsLayout/{showId}")]
        public async Task<IActionResult> GetLatestSeatsLayout(int showId)
        {
            try
            {
                var result = await _bookingService.GetLatestSeatsLayout(showId);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("initiateBooking")]
        public async Task<ActionResult<InitiateBookingResponse>> InitiateBooking([FromBody] InitiateBookingRequest request)
        {
            try
            {
                var result = await _bookingService.InitiateBooking(request);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("cancelBooking")]
        public async Task<ActionResult> CancelBooking([FromBody] CancelBookingRequest request)
        {
            try
            {
                var result = await _bookingService.CancelBooking(request);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
        
        [HttpPost("createRazorpayOrder")]
        public async Task<ActionResult<CreateOrderResponse>> CreateRazorpayOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var result = await _bookingService.CreateRazorpayOrder(request);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("paymentCallback")]
        public async Task<IActionResult> PaymentCallback(PaymentCallbackRequest request)
        {
            try
            {
                var result = await _bookingService.PaymentCallback(request);
                return Ok(new { message = result });
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("getBookingHistory/{userId}")]
        public async Task<IActionResult> GetBookingHistory(int userId)
        {
            try
            {
                var result = await _bookingService.GetUserBookingHistory(userId);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("getBookingData/{bookingId}")]
        public async Task<ActionResult> GetUserBooking(int bookingId)
        {
            try
            {
                var result = await _bookingService.GetUserBookingData(bookingId);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
