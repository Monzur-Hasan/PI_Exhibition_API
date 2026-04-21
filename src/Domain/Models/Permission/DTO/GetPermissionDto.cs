using Domain.Models.Common;

namespace Domain.Models.Permission.DTO
{
    public class GetPermissionDto : BaseModel
    {
        public long Id { get; set; }
        public string? PermissionKey { get; set; }
        public string? PermissionName { get; set; }
        public string? PermissionID { get; set; }
        public string? UserPermissionID { get; set; }
        public string? UserID { get; set; }
        public string? AgencyEmpID { get; set; }
        public string? AgencyID { get; set; }
        public bool? IsAllowed { get; set; }
        public string? PermissionType { get; set; }   
        public string? RegistrationType { get; set; }   
        public long? MainMenuId { get; set; }
        public string? MainMenuName { get; set; }
        public long? SubMenuId { get; set; }
        public long? ActionId { get; set; }
    }
}
