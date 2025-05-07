namespace TickSyncAPI.Models.Dtos
{
    public class RowDto
    {
        public string RowNumber { get; set; }
        public string SeatType { get; set; }
        public decimal Price { get; set; }
        public List<SeatDto> Seats { get; set; }
    }
}
