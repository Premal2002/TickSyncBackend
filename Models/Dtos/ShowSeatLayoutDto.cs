namespace TickSyncAPI.Models.Dtos
{
    public class ShowSeatLayoutDto
    {
        public int ShowId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly ShowTime { get; set; }
        public int VenueId { get; set; }
        public List<SeatTypeGroupDto> seatTypeGroup { get; set; }
    }

}
