using Domain.Models.Permission.DTO;
using Domain.Models.Permission.Filter;
using Domain.OtherModels.Response;
using Infrastructure.Features.Repository.Administrator;
using Infrastructure.Features.Service.Administrator;


namespace Application.Features.Service.Administrator
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository _userPermissionRepository;
        public UserPermissionService(IUserPermissionRepository userPermissionRepository)
        {
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task<IEnumerable<GetPermissionDto>> GetActivePermissionsAsync(UserPermission_Filter userPermission_Filter)
        {
           return await _userPermissionRepository.GetActivePermissionsAsync(userPermission_Filter);
        }

        public async Task<ExecutionStatus> SaveMenuPermissionAsync(UserPermissionRequest request)
        {
            
            var data = await _userPermissionRepository.SaveMenuPermissionAsync(request);
            if (data.Status)
            {
                return new ExecutionStatus
                {
                    Status = true,
                    Msg = "Success",
                    StatusCode = "200"
                };
            }

            return new ExecutionStatus
            {
                Status = false,
                StatusCode = "400"
            };
        }

        public async Task<IEnumerable<GetPermissionDto>> GetAppUserMenusAsync(string? userName)
        {
            return await _userPermissionRepository.GetAppUserMenusAsync(userName);
        }
    }
}
