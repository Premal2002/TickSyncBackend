using Microsoft.EntityFrameworkCore;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Dtos;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using TickSyncAPI.Dtos.Booking;
using Razorpay.Api;

namespace TickSyncAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingSystemContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public BookingService(BookingSystemContext context, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _context = context;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<ShowSeatLayoutDto> GetLatestSeatsLayout(int showId)
        {
            var show = await _context.Shows
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShowId == showId);
            var venue = await _context.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.VenueId == show.VenueId);
            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MovieId == show.MovieId);


            if (show == null)
                throw new CustomException(404, "Show not found.");

            var allSeats = await _context.Seats
                .AsNoTracking()
                .Where(seat => seat.VenueId == show.VenueId)
                .ToListAsync();

            var bookedSeatIds = await _context.BookingSeats
                .Where(bs => bs.Booking.ShowId == showId && bs.Booking.Status == "Confirmed")
                .Select(bs => bs.SeatId)
                .ToListAsync();

            var seatTypeGroups = allSeats
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
                                .Select(seat =>
                                {
                                    string status = "Available";
                                    string cacheKey = $"seat_lock:{showId}:{seat.SeatId}";
                                    bool isLocked = _memoryCache.TryGetValue<SeatLockInfo>(cacheKey, out var lockInfo);

                                    if (bookedSeatIds.Contains(seat.SeatId))
                                        status = "Booked";
                                    else if (isLocked && lockInfo.ExpiresAt > DateTime.UtcNow)
                                        status = "Locked";

                                    return new SeatDto
                                    {
                                        SeatId = seat.SeatId,
                                        RowNumber = seat.RowNumber,
                                        SeatNumber = seat.SeatNumber,
                                        SeatType = seat.SeatType,
                                        Status = status,
                                        Price = seat.SeatType == "Premium" ? show.PremiumSeatPrice : show.RegularSeatPrice
                                    };
                                })
                                .OrderBy(seat => seat.SeatId)
                                .ToList()
                        })
                        .OrderBy(row => row.RowNumber)
                        .ToList()
                })
                .OrderBy(s => s.SeatType)
                .ToList();

            return new ShowSeatLayoutDto
            {
                ShowId = show.ShowId,
                ShowDate = show.ShowDate,
                ShowTime = show.ShowTime,
                VenueId = show.VenueId,
                MovieName = movie.Title,
                VenueName = venue.Name,
                VenueLocation = venue.Location,
                seatTypeGroup = seatTypeGroups
            };
        }

        public async Task<List<int>> LockSeatsInCache(SeatLockRequest request)
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

            var bookedSeatIds = await _context.BookingSeats
                .Where(bs => request.SeatIds.Contains(bs.SeatId)
                             && bs.Booking.ShowId == request.ShowId
                             && bs.Booking.Status == "Confirmed")
                .Select(bs => bs.SeatId)
                .ToListAsync();

            if (bookedSeatIds.Any())
            {
                throw new CustomException(409, $"Seats already booked: {string.Join(",", bookedSeatIds)}");
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
                    LockedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(10)
                };

                _memoryCache.Set(cacheKey, lockInfo, TimeSpan.FromMinutes(10));
            }
            return request.SeatIds;
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
                ReferenceId = "",
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

        //public async Task<ConfirmBookingResponse> ConfirmBooking(ConfirmBookingRequest request)
        //{
        //    var booking = await _context.Bookings
        //        .Include(b => b.BookingSeats)
        //        .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

        //    if (booking == null)
        //        throw new CustomException(409, $"Booking not found.");

        //    if (booking.Status == "Confirmed")
        //        throw new CustomException(400, $"Booking is already confirmed.");

        //    if (booking.Status == "Cancelled")
        //        throw new CustomException(400, $"Booking is cancelled earlier.");

        //    // Confirm booking
        //    booking.Status = "Confirmed";
        //    _context.Bookings.Update(booking);

        //    var seatIds = booking.BookingSeats.Select(bs => bs.SeatId).ToList();

        //    // Remove seat locks from memory cache
        //    foreach (var seatId in seatIds)
        //    {
        //        var cacheKey = $"seat_lock:{booking.ShowId}:{seatId}";
        //        _memoryCache.Remove(cacheKey);
        //    }

        //    await _context.SaveChangesAsync();

        //    return new ConfirmBookingResponse
        //    {
        //        BookingId = booking.BookingId,
        //        Status = booking.Status
        //    };
        //}

        public async Task<bool> CancelBooking(CancelBookingRequest request)
        {
            var booking = await _context.Bookings
               .Include(b => b.BookingSeats)
               .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            if (booking == null)
                throw new CustomException(409, $"Booking not found.");

            if (booking.Status == "Confirmed")
                throw new CustomException(400, $"Booking is already confirmed.");

            booking.Status = "Cancelled";
            _context.Bookings.Update(booking);

            var seatIds = booking.BookingSeats.Select(bs => bs.SeatId).ToList();

            // Remove seat locks from memory cache
            foreach (var seatId in seatIds)
            {
                var cacheKey = $"seat_lock:{booking.ShowId}:{seatId}";
                _memoryCache.Remove(cacheKey);
            }

            _context.BookingSeats.RemoveRange(booking.BookingSeats);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserBookingsResponse>> GetUserBookings(int userId)
        {
            var userBookings = await _context.Bookings
                    .Where(b => b.UserId == userId && b.Status != "Cancelled")
                    .Join(
                        _context.Shows,
                        booking => booking.ShowId,
                        show => show.ShowId,
                        (booking, show) => new UserBookingsResponse
                        {
                            BookingId = booking.BookingId,
                            Status = booking.Status,
                            TotalAmount = booking.TotalAmount,
                            ShowTime = show.ShowTime
                        }
                    )
                    .ToListAsync();

            return userBookings;
        }

        public async Task<CreateOrderResponse> CreateRazorpayOrder(CreateOrderRequest request)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.Status == "Pending");

            if (booking == null)
            {
                throw new CustomException(404, "Invalid or already processed booking.");
            }

            var client = new RazorpayClient(_configuration["Razorpay:Key"], _configuration["Razorpay:Secret"]);

            Dictionary<string, object> options = new()
            {
                { "amount", (int)(request.Amount * 100) }, // Razorpay accepts amount in paise
                { "currency", "INR" },
                { "receipt", "booking_rcpt_" + request.BookingId },
                { "payment_capture", 1 }
            };

            Order order = client.Order.Create(options);

            booking.Status = "PaymentInitiated";
            booking.ReferenceId = order["id"]; 
            await _context.SaveChangesAsync();

            return new CreateOrderResponse
            {
                BookingId = booking.BookingId,
                TotalAmount = request.Amount,
                OrderId = order["id"].ToString(),
                RazorpayKey = _configuration["Razorpay:Key"]
            };
        }

        public async Task<string> PaymentCallback(PaymentCallbackRequest request)
        {
            var booking = await _context.Bookings
                                 .Include(b => b.Payments)
                                 .Include(b => b.BookingSeats)
                                 .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            if (booking == null)
                throw new CustomException(404, "Booking not found.");

            if (booking.Status == "Confirmed")
                return "Already confirmed.";

            var secret = _configuration["Razorpay:Secret"];

            bool isValid = RazorpayUtils.VerifyPaymentSignature(
                request.RazorpayOrderId,
                request.RazorpayPaymentId,
                request.RazorpaySignature,
                secret
            );

            if (!isValid)
            {
                booking.Status = "Failed";
                await _context.SaveChangesAsync();
                throw new CustomException(400, "Invalid payment signature.");
            }

            // ✅ Confirm booking
            booking.Status = "Confirmed";

            // ✅ Add Payment record
            booking.Payments.Add(new Models.Payment
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                Amount = booking.TotalAmount,
                Currency = "INR",
                Status = "Success",
                RazorpayOrderId = request.RazorpayOrderId,
                RazorpayPaymentId = request.RazorpayPaymentId,
                RazorpaySignature = request.RazorpaySignature,
                TransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                PaymentMethod = "Razorpay",
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Notes = "Payment confirmed via frontend callback."
            });

            // ✅ Unlock seats
            foreach (var seat in booking.BookingSeats)
            {
                var cacheKey = $"seat_lock:{booking.ShowId}:{seat.SeatId}";
                _memoryCache.Remove(cacheKey);
            }

            await _context.SaveChangesAsync();

            return "Booking confirmed successfully.";
        }
    }
}
