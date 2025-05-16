namespace TickSyncAPI.Dtos.Seat
{
    public class SeatLockInfo
    {
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public int SeatId { get; set; }
        public DateTime LockedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
