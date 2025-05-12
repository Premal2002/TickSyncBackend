namespace TickSyncAPI.Dtos.Booking
{
    public class PaymentCallbackRequest
    {
        public int BookingId { get; set; }
        public string RazorpayPaymentId { get; set; } = null!;
        public string RazorpayOrderId { get; set; } = null!;
        public string RazorpaySignature { get; set; } = null!;
    }

}
