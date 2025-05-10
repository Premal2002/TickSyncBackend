namespace TickSyncAPI.Dtos.Seat
{
    public class SeatTypeGroupDto
    {
        public string SeatType { get; set; }
        public decimal Price { get; set; }
        public List<RowDto> Rows { get; set; }
    }

}
