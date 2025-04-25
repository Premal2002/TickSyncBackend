using System.ComponentModel.DataAnnotations;

namespace TickSyncAPI.Models.Dtos
{
    public class AddUserRole
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
