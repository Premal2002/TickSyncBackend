namespace TickSyncAPI.Models.Dtos
{
    public class InitiateBookingRequest
    {
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
