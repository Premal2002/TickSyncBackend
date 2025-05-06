namespace TickSyncAPI.Models.Dtos
{
    public class VerifyResetCodeRequest
    {
        public string Email { get; set; }
        public string SecretCode { get; set; }
    }
}
