namespace TickSyncAPI.Models.Dtos
{
    public class TokenDto
    {
        public int UserId { get; set; }

        public string JwtToken { get; set; } = null!;
    }
}
