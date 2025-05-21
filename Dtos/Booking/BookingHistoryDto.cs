using TickSyncAPI.Dtos.Seat;

namespace TickSyncAPI.Dtos.Booking
{
    public class BookingHistoryDto
    {
        public int BookingId { get; set; }
        public string ReferenceId { get; set; }
        public string MovieName { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly ShowTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<SeatDto> Seats { get; set; } = new();
    }


}
