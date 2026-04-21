
using Domain.Models.Access.DTO;
using Domain.Models.Administrator.DomainModel;
using Domain.OtherModels.Response;

namespace Infrastructure.Features.Service.Administrator
{
    public interface IMenuService
    {
        Task<ExecutionStatus> DeleteMenuAsync(string Id, string TableName);
        Task<ExecutionStatus> SaveMainMenuAsync(MainMenuInputDto menu);
        Task<DBResponse<MainMenu>> GetMainMenus(MainMenuFilter filter);
        Task<ExecutionStatus> UpdateMainMenuAsync(UpdateMainMenuDto menu);

        Task<ExecutionStatus> SaveSubMenuAsync(SubmenuDto submenu);

        Task<DBResponse<SubMenu>> GetSubMenus(SubMenuFilter filter);
        Task<ExecutionStatus> UpdateSubMenuAsync(UpdateSubMenu submenu);
        List<MenuStructureDto> GetMenuHierarchy();
    }
}
