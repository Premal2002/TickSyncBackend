namespace TickSyncAPI.Dtos.Booking
{
    public class CreateOrderRequest
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
    }

}
