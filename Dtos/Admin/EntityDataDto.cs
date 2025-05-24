namespace TickSyncAPI.Dtos.Admin
{
    public class EntityDataDto<T>
    {
        public string Title { get; set; }
        public List<T> EntityData {get; set; }
    }
}
