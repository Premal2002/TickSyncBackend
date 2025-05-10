using System.ComponentModel.DataAnnotations;

namespace TickSyncAPI.Dtos.Auth
{
    public class AddUserRole
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
