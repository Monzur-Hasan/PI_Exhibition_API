
using Domain.Models.Access.DTO;
using Domain.Models.Administrator.DomainModel;
using Domain.OtherModels.Response;

namespace Infrastructure.Features.Repository.Administrator
{
    public interface IMenuRepository
    {
        Task<ExecutionStatus> DeleteMenuAsync(string Id, string TableName);
        Task<DBResponse<MainMenu>> GetMainMenusAsync(MainMenuFilter filter);
        Task<MainMenu> GetMainMenuByIdAync(string MMId);
        Task<bool> InsertMainMenuAsync(MainMenuInputDto menu);
        Task<bool> IsMainMenuExistsAsync(string ShortName, string MenuName);
        Task<bool> IsMainMenuExistsForUpdateAsync(string MenuName, string MMId);
        Task<ExecutionStatus> UpdateMainMenuTypeAsync(string MMId, Dictionary<string, object> changedFields);

        Task<DBResponse<SubMenu>> GetSubMenusAsync(SubMenuFilter filter);
        Task<SubMenu> GetSubMenuByIdAync(string SubmenuId);
        Task<bool> InsertSubMenuAsync(SubmenuDto submenu);
        Task<bool> IsSubMenuExistsAsync(string SubmenuName, string Path);
        Task<ExecutionStatus> UpdateSubMenuTypeAsync(string SubmenuId, Dictionary<string, object> changedFields);
        List<MenuStructureDto> GetMenuHierarchy();
    }
}
