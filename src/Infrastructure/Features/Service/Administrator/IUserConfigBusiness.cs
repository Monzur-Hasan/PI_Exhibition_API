using Domain.Models.Access.DomainModel;
using Domain.Models.Access.DTO;
using Domain.Models.Administrator.ViewModel;
using Domain.OtherModels.Response;
using Domain.ViewModels.OTP;
using Domain.ViewModels.UserManagement;

namespace Infrastructure.Features.Service.Administrator
{
    public interface IUserConfigBusiness
    {
        Task<IEnumerable<AppUserMenu>> GetAppMenusAsync(string userId, string roleId, long companyId, long organizationId, string flag);
        Task<IEnumerable<ApplicationUserViewModel>> GetApplicationUsers(string fullName, string userName, string roleId, string address, string email, string phoneNumber, long branchId, long divisionId, long companyId, long organizationId, string fromDate, string toDate);
        Task<IEnumerable<AppMainMenuForPermission>> GetAppMenusForPermissionAsync(string userId, string roleId, long companyId, long organizationId, string flag);
        Task<ExecutionStatus> SaveUserInfo(UserInfoData userInfo, long BranchId, long ComId, long OrgId, string UserId);
        Task<ExecutionStatus> UserValidator(ApplicationUserViewModel AppUserInfo);
        Task<ExecutionStatus> SaveRoleAuthAsync(List<AppMainMenuForPermission> appMainMenus, string roleId, long branchId, long companyId, long organizationId, string userId);
        Task<bool> AssignUserMenuAsync(List<AppMainMenuForPermission> appMainMenuForPermissions, ApplicationUser appUser, long BranchId, long ComId, long OrgId, string UserId);
        Task<bool> AssignRoleMenuAsync(List<AppMainMenuForPermission> appMainMenus, string roleId, long branchId, long companyId, long organizationId, string userId);
        Task<IEnumerable<AppUserMenu>> GetAppUserMenusAsync(string username);
        Task<UsermenuWithComponent> GetAppUsermenuWithComponentAsync(string username);
        Task<ExecutionStatus> CheckUserAsync(string userId, string currentPassword);
        Task<ExecutionStatus> ChangeUserDefaultPasswordAsync(ChangePasswordViewModel model);
        Task<ExecutionStatus> ChangeUserPasswordAsync(ChangePasswordViewModel model);
        Task<Component> CheckUserprivilegeAsync(Usercomponentprivilege usercomponentprivilege);
        Task GetUserInfosAsync(long companyId, long organizationId);
        Task<ApplicationUser> GetUserInfoByIdAysc(string id);
    }
}
