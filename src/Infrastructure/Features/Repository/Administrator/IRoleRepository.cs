using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Filter;
using Domain.OtherModels.Response;

namespace Infrastructure.Features.Repository.Administrator
{
    public interface IRoleRepository
    {
        Task<DBResponse<GetApplicationRoleDto>> GetAllRolesAsync(ApplicationRole_Filter filter);
        Task<IEnumerable<GetRolePermissionDto>> GetActivePermissionsAsync(ActivePermissions_Filter filter);
        Task<ExecutionResponse<CreateRoleResultDto>> CreateRoleAsync(CreateRoleRequestDto createRoleDto);
        Task<ExecutionResponse<List<string>>> GetRolePermissionsAsync(string[] roleIds);
        Task<ExecutionResponse<string>> UpdateRoleAsync(UpdateRoleRequestDto updateRoleDto);
        Task<ExecutionResponse<string>> DeleteRoleAsync(DeleteRoleRequestDto deleteRoleRequestDto);
        Task<IEnumerable<GetRolePermissionDto>> GetRolePermissionMenusAsync(string? userName); 
        Task<(bool found, Guid roleId)> TryGetRoleIdAsync(string registrationType);
    }
}
