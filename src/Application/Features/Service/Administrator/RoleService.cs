using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Filter;
using Domain.OtherModels.Response;
using Infrastructure.Features.Repository.Administrator;
using Infrastructure.Features.Service.Administrator;

namespace Application.Features.Service.Administrator
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }


        public async Task<DBResponse<GetApplicationRoleDto>> GetAllRolesAsync(ApplicationRole_Filter filter)
        {
            DBResponse<GetApplicationRoleDto> data = new DBResponse<GetApplicationRoleDto>();
            try
            {
                data = await _roleRepository.GetAllRolesAsync(filter);
            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<IEnumerable<GetRolePermissionDto>> GetActivePermissionsAsync(ActivePermissions_Filter filter)
        {
            return await _roleRepository.GetActivePermissionsAsync(filter);
        }

        public async Task<ExecutionResponse<CreateRoleResultDto>> CreateRoleAsync(CreateRoleRequestDto createRoleDto)
        {
            return await _roleRepository.CreateRoleAsync(createRoleDto);
        }

        public async Task<ExecutionResponse<List<string>>> GetRolePermissionsAsync(string[] roleIds)
        {
            return await _roleRepository.GetRolePermissionsAsync(roleIds);
        }

        public async Task<ExecutionResponse<string>> UpdateRoleAsync(UpdateRoleRequestDto updateRoleDto)
        {
            return await _roleRepository.UpdateRoleAsync(updateRoleDto);
        }
        public async Task<ExecutionResponse<string>> DeleteRoleAsync(DeleteRoleRequestDto deleteRoleRequestDto)
        {
            return await _roleRepository.DeleteRoleAsync(deleteRoleRequestDto);
        }


        public async Task<IEnumerable<GetRolePermissionDto>> GetRolePermissionMenusAsync(string? userName)
        {
            return await _roleRepository.GetRolePermissionMenusAsync(userName);
        }
    }
}



