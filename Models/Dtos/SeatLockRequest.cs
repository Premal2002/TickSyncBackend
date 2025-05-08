namespace TickSyncAPI.Models.Dtos
{
    public class SeatLockRequest
    {
        public int ShowId { get; set; }
        public int UserId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
