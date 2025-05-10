namespace TickSyncAPI.Dtos.Auth
{
    public class VerifyResetCodeRequest
    {
        public string Email { get; set; }
        public string SecretCode { get; set; }
    }
}
