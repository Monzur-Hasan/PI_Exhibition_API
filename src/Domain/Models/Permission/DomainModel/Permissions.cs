using Domain.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Permission.DomainModel
{
    public class Permissions : BaseModel
    {
        public long ID { get; set; }
        public string? PermissionID { get; set; }
        public long? MainMenuID { get; set; }
        public long? SubMenuID { get; set; }
        public string? PermissionKey { get; set; }
        public string? PermissionName { get; set; }
        public string? MainMenuName { get; set; }
        public string? PermissionType { get; set; }
        public long? ActionId { get; set; }
      
    }
}
