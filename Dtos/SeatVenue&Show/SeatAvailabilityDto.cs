namespace TickSyncAPI.Dtos.Seat
{
    public class SeatAvailabilityDto
    {
        public int SeatId { get; set; }
        public string RowNumber { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; 
    }

}
