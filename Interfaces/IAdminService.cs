using TickSyncAPI.Dtos.Admin;
using TickSyncAPI.Dtos.Seat;
using TickSyncAPI.Models;

namespace TickSyncAPI.Interfaces
{
    public interface IAdminService
    {
        public Task<List<object>> GetAllConfirmedBookings();
        public Task<List<object>> GetAllPayments();
        public Task<AllDataCountDto> GetAllDataCounts();
        public Task<EntityDataDto<object>> GetEntityData(string entity);

    }
}
