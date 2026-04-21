using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Filter;
using Domain.OtherModels.Response;

namespace Infrastructure.Features.Service.Administrator
{
    public interface IRoleService
    {
        Task<DBResponse<GetApplicationRoleDto>> GetAllRolesAsync(ApplicationRole_Filter filter);
        Task<IEnumerable<GetRolePermissionDto>> GetActivePermissionsAsync(ActivePermissions_Filter filter);
        Task<ExecutionResponse<CreateRoleResultDto>> CreateRoleAsync(CreateRoleRequestDto createRoleDto);
        Task<ExecutionResponse<List<string>>> GetRolePermissionsAsync(string[] roleIds);
        Task<ExecutionResponse<string>> UpdateRoleAsync(UpdateRoleRequestDto updateRoleDto);
        Task<ExecutionResponse<string>> DeleteRoleAsync(DeleteRoleRequestDto deleteRoleRequestDto);
        Task<IEnumerable<GetRolePermissionDto>> GetRolePermissionMenusAsync(string? userName);
    }
}
