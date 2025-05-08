namespace TickSyncAPI.Models.Dtos
{
    public class SeatLockInfo
    {
        public int UserId { get; set; }
        public DateTime LockedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
