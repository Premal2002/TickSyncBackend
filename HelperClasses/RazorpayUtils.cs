using System.Text;

namespace TickSyncAPI.HelperClasses
{
    public class RazorpayUtils
    {
        public static bool VerifyPaymentSignature(string orderId, string paymentId, string signature, string secret)
        {
            string payload = orderId + "|" + paymentId;

            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower(); // ✅ Razorpay uses Base64
                return generatedSignature == signature;
            }
        }

    }
}
