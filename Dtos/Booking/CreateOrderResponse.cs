namespace TickSyncAPI.Dtos.Booking
{
    public class CreateOrderResponse
    {
        public string OrderId { get; set; }
        public int BookingId { get; set; }
        public string RazorpayKey { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
