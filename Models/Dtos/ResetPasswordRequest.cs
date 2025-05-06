namespace TickSyncAPI.Models.Dtos
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string SecretCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
