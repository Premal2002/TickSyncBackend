namespace TickSyncAPI.Dtos.Seat
{
    public class ShowSeatLayoutDto
    {
        public int ShowId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly ShowTime { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string MovieName { get; set; }
        public string VenueLocation { get; set; }
        public List<SeatTypeGroupDto> seatTypeGroup { get; set; }
    }

}
