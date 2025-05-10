using TickSyncAPI.Models;

namespace TickSyncAPI.Dtos.Seat
{
    public class ShowVenueGroup
    {
        public int VenueId { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public List<Show> Shows { get; set; }
    }

}
