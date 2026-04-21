using Domain.Models.Administrator.ViewModel;
using Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Access.DTO
{
    public class UserInfoData
    {
        public ApplicationUserViewModel AppUserInfo { get; set; }
        public List<AppMainMenuForPermission> AppUserMenuPermission { get; set; }
    }
    public class RoleAuthData
    {
        [Range(1, long.MaxValue)]
        public long OrganizationId { get; set; }
        [Range(1, long.MaxValue)]
        public long CompanyId { get; set; }
        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }
        public List<AppMainMenuForPermission> AppUserMenuPermission { get; set; }
    }
    /// <summary>
    /// User Customer Authorization Model
    /// </summary>
    public class UserAuthorizationViewModel : BaseModel
    {
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
        public bool Report { get; set; }
        public bool Check { get; set; }
        public bool Accept { get; set; }
        public bool Upload { get; set; }
        public long DivisionId { get; set; }
        public long BranchId { get; set; }
    }
    /// <summary>
    /// User Customer-Tab Authorization Model
    /// </summary>
    public class UserAuthTabViewModel : BaseModel
    {
        public long UATId { get; set; }
        public long TaskId { get; set; }
        public string UserId { get; set; }
        public long SubmenuId { get; set; }
        public long TabId { get; set; }
        [StringLength(100)]
        public string TabName { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Check { get; set; }
        public bool Accept { get; set; }
        public bool Upload { get; set; }
        public long BranchId { get; set; }
    }
    /// <summary>
    /// User Role Authorization Model
    /// </summary>
    public class RoleAuthorizationViewModel : BaseModel
    {
        public long TaskId { get; set; }
        public string RoleId { get; set; }
        public long ModuleId { get; set; }
        public long MainmenuId { get; set; }
        public long SubmenuId { get; set; }
        public long ParentSubmenuId { get; set; }
        public bool HasTab { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Check { get; set; }
        public bool Accept { get; set; }
        public bool Upload { get; set; }
        public long BranchId { get; set; }
    }
    /// <summary>
    /// User Role-Tab Authorization Model
    /// </summary>
    public class RoleAuthTabViewModel : BaseModel
    {
        public long RATId { get; set; }
        public long TaskId { get; set; }
        [StringLength(256)]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public long SubmenuId { get; set; }
        public long TabId { get; set; }
        [StringLength(100)]
        public string TabName { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Report { get; set; }
        public bool Check { get; set; }
        public bool Accept { get; set; }
        public bool Upload { get; set; }
        public long BranchId { get; set; }
    }
}
