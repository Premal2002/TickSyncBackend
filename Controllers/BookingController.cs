using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Dtos;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;

namespace TickSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("getLatestSeatsLayout")]
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

        [HttpPost("confirmBooking")]
        public async Task<ActionResult> ConfirmBooking([FromBody] ConfirmBookingRequest request)
        {
            try
            {
                var result = await _bookingService.ConfirmBooking(request);
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
