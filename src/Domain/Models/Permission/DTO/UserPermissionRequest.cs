using Domain.Models.Permission.DomainModel;
using Domain.Models.Common;

namespace Domain.Models.Permission.DTO
{
    public class UserPermissionRequest : BaseModel
    {    
        public long ID { get; set; }
        public string? UserPermissionID { get; set; }
        public string? AgencyEmpID { get; set; }
        public string? AgencyID { get; set; }       
        public string? UserID { get; set; }
        public List<UserPermission> Permissions { get; set; } = new();

    }
}
