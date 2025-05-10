namespace TickSyncAPI.Dtos
{
    public class InitiateBookingResponse
    {
        public int BookingId { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
    }
}
