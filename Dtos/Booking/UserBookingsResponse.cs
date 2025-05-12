namespace TickSyncAPI.Dtos.Booking
{
    public class UserBookingsResponse
    {
        public int BookingId { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Status { get; set; }

        public TimeOnly ShowTime { get; set; }
    }
}
