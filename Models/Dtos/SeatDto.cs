namespace TickSyncAPI.Models.Dtos
{
    public class SeatDto
    {
        public int SeatId { get; set; }
        public string RowNumber { get; set; }
        public string SeatNumber { get; set; }
        public string SeatType { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}
