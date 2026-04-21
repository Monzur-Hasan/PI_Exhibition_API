using Domain.Models.Permission.DTO;
using Domain.Models.Permission.Filter;
using Domain.OtherModels.Response;

namespace Infrastructure.Features.Service.Administrator
{
    public interface IUserPermissionService
    {
        Task<IEnumerable<GetPermissionDto>> GetActivePermissionsAsync(UserPermission_Filter userPermission_Filter);
        Task<ExecutionStatus> SaveMenuPermissionAsync(UserPermissionRequest request);
        Task<IEnumerable<GetPermissionDto>> GetAppUserMenusAsync(string? userName);
    }
}
