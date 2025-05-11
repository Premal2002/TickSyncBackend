namespace TickSyncAPI.Dtos.Seat
{
    public class SeatLockInfo
    {
        public int UserId { get; set; }
        public DateTime LockedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
