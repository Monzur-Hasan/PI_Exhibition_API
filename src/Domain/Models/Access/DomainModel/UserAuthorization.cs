using Domain.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Access.DomainModel
{
    public class UserAuthorization : BaseModel
    {
        [Key]
        public long TaskId { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public long ModuleId { get; set; }
        public long MainmenuId { get; set; }
        public long SubmenuId { get; set; }
        public long ParentSubmenuId { get; set; }
        public bool IsSubmenuPermission { get; set; }
        public bool IsPageTabPermission { get; set; }
        public bool HasTab { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Check { get; set; }
        public bool Accept { get; set; }
        public bool Report { get; set; }
        public bool Upload { get; set; }
        public long DivisionId { get; set; }
        public long BranchId { get; set; }
        public long CompanyId { get; set; }
        public long OrganizationId { get; set; }

    }
}
