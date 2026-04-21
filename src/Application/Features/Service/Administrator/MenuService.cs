using Domain.Models.Access.DTO;
using Domain.Models.Administrator.DomainModel;
using Domain.OtherModels.Response;
using Infrastructure.Features.Repository.Administrator;
using Infrastructure.Features.Service.Administrator;
using System.Net;
namespace Application.Features.Service.Administrator
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        

        //Main Menu services

        public async Task<DBResponse<MainMenu>> GetMainMenus(MainMenuFilter filter)
        {
            return await _menuRepository.GetMainMenusAsync(filter);
        }

        public async Task<ExecutionStatus> SaveMainMenuAsync(MainMenuInputDto menu)
        {
            if (await _menuRepository.IsMainMenuExistsAsync(menu.ShortName, menu.MenuName))
            {
                return new ExecutionStatus
                {
                    Status = false,
                    IsDuplicateContact = true,
                    Msg = "Main Menu already exists."
                };
            }

            bool isSaved = await _menuRepository.InsertMainMenuAsync(menu);

            return new ExecutionStatus
            {
                Status = isSaved,
                StatusCode = isSaved ? "200" : "400",
                Msg = isSaved ? "Main Menu successfully saved." : "Failed to save Main Menu."
            };

        }

        public async Task<ExecutionStatus> UpdateMainMenuAsync(UpdateMainMenuDto menu)
        {
            // Check if the new name already exists (excluding the current record)
            var existingType = await _menuRepository.IsMainMenuExistsForUpdateAsync(menu.MenuName, menu.MMId);
            if (existingType)
            {
                return new ExecutionStatus
                {
                    Status = false,
                    ErrorMsg = "This name already exists!"
                };
            }
            // Fetch existing entity
            var existingEntity = await _menuRepository.GetMainMenuByIdAync(menu.MMId);
            if (existingEntity == null)
            {
                return new ExecutionStatus
                {
                    Status = false,
                    ErrorMsg = "Main Menu not found."
                };
            }

            // Track changed fields
            var changedFields = new Dictionary<string, object>();
            var dto = menu;


            if (dto.MenuName != null &&  dto.MenuName != existingEntity.MenuName)
                changedFields[nameof(MainMenu.MenuName)] = dto.MenuName;

            if (dto.ShortName!= null && dto.ShortName != existingEntity.ShortName)
                changedFields[nameof(MainMenu.ShortName)] = dto.ShortName;

            if (dto.IconClass != null &&  dto.IconClass != existingEntity.IconClass)
                changedFields[nameof(MainMenu.IconClass)] = dto.IconClass;

            if (dto.IconColor != null &&  dto.IconColor != existingEntity.IconColor)
                changedFields[nameof(MainMenu.IconColor)] = dto.IconColor;

            if (dto.ApplicationId != null &&  dto.ApplicationId != existingEntity.ApplicationId)
                changedFields[nameof(MainMenu.ApplicationId)] = dto.ApplicationId;

            if (dto.IsActive != null &&  dto.IsActive != existingEntity.IsActive)
                changedFields[nameof(MainMenu.IsActive)] = dto.IsActive;

            if (dto.SequenceNo != null &&  dto.SequenceNo != existingEntity.SequenceNo)
                changedFields[nameof(MainMenu.SequenceNo)] = dto.SequenceNo;

            // Add server-side fields (e.g., UpdatedDate)
            changedFields[nameof(MainMenu.UpdatedDate)] = DateTime.UtcNow;

            // Call repository to update the complain
            var executionStatus = await _menuRepository.UpdateMainMenuTypeAsync(menu.MMId, changedFields);

            return executionStatus;
        }

        //Sub Menu services

        public async Task<DBResponse<SubMenu>> GetSubMenus(SubMenuFilter filter)
        {
            return await _menuRepository.GetSubMenusAsync(filter);
        }

        public async Task<ExecutionStatus> SaveSubMenuAsync(SubmenuDto submenu)
        {
            if (await _menuRepository.IsSubMenuExistsAsync(submenu.SubmenuName, submenu.Path))
            {
                return new ExecutionStatus
                {
                    Status = false,
                    IsDuplicateContact = true,
                    Msg = "Sub Menu already exists."
                };
            }

            bool isSaved = await _menuRepository.InsertSubMenuAsync(submenu);

            return new ExecutionStatus
            {
                Status = isSaved,
                StatusCode = isSaved ? "200" : "400",
                Msg = isSaved ? "Sub Menu successfully saved." : "Failed to save Sub Menu."
            };

        }

        public async Task<ExecutionStatus> UpdateSubMenuAsync(UpdateSubMenu submenu)
        {
            // Fetch existing entity
            var existingEntity = await _menuRepository.GetSubMenuByIdAync(submenu.SubmenuId);
            if (existingEntity == null)
            {
                int statusCodeValue = (int)HttpStatusCode.NotFound;
                string statusCodeString = statusCodeValue.ToString();
                return new ExecutionStatus
                {
                    Status = false,
                    ErrorMsg = "Sub Menu not found.",
                    Code = statusCodeString,
                    Msg = "Sub Menu not found.",
                };
            }

            // Check if the new name already exists (excluding the current record)
            var existingType = await _menuRepository.IsSubMenuExistsAsync(submenu.SubmenuName, submenu.Path);
            if (existingType)
            {
                int statusCodeValue = (int)HttpStatusCode.Conflict;
                string statusCodeString = statusCodeValue.ToString();
                return new ExecutionStatus
                {
                    Status = false,
                    Msg = "This name or path already exists!",
                    IsDuplicate = true,
                    Code = statusCodeString,

                };
            }

            // Track changed fields
            var changedFields = new Dictionary<string, object>();
            var dto = submenu;

            if (dto.SubmenuName != null && dto.SubmenuName != existingEntity.SubmenuName)
                changedFields[nameof(UpdateSubMenu.SubmenuName)] = dto.SubmenuName;

            if (dto.ControllerName != null && dto.ControllerName != existingEntity.ControllerName)
                changedFields[nameof(UpdateSubMenu.ControllerName)] = dto.ControllerName;

            if (dto.ActionName != null && dto.ActionName != existingEntity.ActionName)
                changedFields[nameof(UpdateSubMenu.ActionName)] = dto.ActionName;

            if (dto.Path != null && dto.Path != existingEntity.Path)
                changedFields[nameof(UpdateSubMenu.Path)] = dto.Path;

            if (dto.Component != null && dto.Component != existingEntity.Component)
                changedFields[nameof(UpdateSubMenu.Component)] = dto.Component;

            if (dto.IconClass != null && dto.IconClass != existingEntity.IconClass)
                changedFields[nameof(UpdateSubMenu.IconClass)] = dto.IconClass;

            if (dto.IconColor != null && dto.IconColor != existingEntity.IconColor)
                changedFields[nameof(UpdateSubMenu.IconColor)] = dto.IconColor;

            if (dto.IsViewable != null && dto.IsViewable != existingEntity.IsViewable)
                changedFields[nameof(UpdateSubMenu.IsViewable)] = dto.IsViewable;

            if (dto.IsActAsParent != null && dto.IsActAsParent != existingEntity.IsActAsParent)
                changedFields[nameof(UpdateSubMenu.IsActAsParent)] = dto.IsActAsParent;

            if (dto.HasTab != null && dto.HasTab != existingEntity.HasTab)
                changedFields[nameof(UpdateSubMenu.HasTab)] = dto.HasTab;

            if (dto.ParentSubmenuId != null && dto.ParentSubmenuId != existingEntity.ParentSubmenuId)
                changedFields[nameof(UpdateSubMenu.ParentSubmenuId)] = dto.ParentSubmenuId;

            if (dto.IsActive != null && dto.IsActive != existingEntity.IsActive)
                changedFields[nameof(UpdateSubMenu.IsActive)] = dto.IsActive;

            if (dto.MMId != null && dto.MMId != existingEntity.MMId)
                changedFields[nameof(UpdateSubMenu.MMId)] = dto.MMId;

            if (dto.ModuleId != null && dto.ModuleId != existingEntity.ModuleId)
                changedFields[nameof(UpdateSubMenu.ModuleId)] = dto.ModuleId;

            if (dto.ApplicationId != null && dto.ApplicationId != existingEntity.ApplicationId)
                changedFields[nameof(UpdateSubMenu.ApplicationId)] = dto.ApplicationId;

            if (dto.UpdatedBy != null && dto.UpdatedBy != existingEntity.UpdatedBy)
                changedFields[nameof(UpdateSubMenu.UpdatedBy)] = dto.UpdatedBy;

            if (dto.MenuSequence != null && dto.MenuSequence != existingEntity.MenuSequence)
                changedFields[nameof(UpdateSubMenu.MenuSequence)] = dto.MenuSequence;

            // Add server-side fields (e.g., UpdatedDate)
            changedFields[nameof(UpdateSubMenu.UpdatedBy)] = "System"; // Example: use a system user


            // Call repository to update the complain
            var executionStatus = await _menuRepository.UpdateMainMenuTypeAsync(submenu.SubmenuId, changedFields);

            return executionStatus;
        }


        //Shared Menu services
        public async Task<ExecutionStatus> DeleteMenuAsync(string Id, string TableName)
        {
            if (TableName == "tblMainMenus")
            {
                var existingEntity = await _menuRepository.GetMainMenuByIdAync(Id);
                // Fetch existing entity
                if (existingEntity == null)
                {
                    int statusCodeValue = (int)HttpStatusCode.NotFound;
                    string statusCodeString = statusCodeValue.ToString();
                    return new ExecutionStatus
                    {
                        Status = false,
                        ErrorMsg = "Main Menu not found.",
                        Code = statusCodeString,
                        Msg = "Main Menu not found.",
                    };
                }
            }
            else if (TableName == "tblSubMenus")
            {
                var existingEntity = await _menuRepository.GetSubMenuByIdAync(Id);
                // Fetch existing entity
                if (existingEntity == null)
                {
                    int statusCodeValue = (int)HttpStatusCode.NotFound;
                    string statusCodeString = statusCodeValue.ToString();
                    return new ExecutionStatus
                    {
                        Status = false,
                        ErrorMsg = "Sub Menu not found.",
                        Code = statusCodeString,
                        Msg = "Sub Menu not found.",
                    };
                }
            }
            else
            {
                return new ExecutionStatus
                {
                    Status = false,
                    Msg = "Invalid table name.",
                    StatusCode = "400"
                };
            }

            return await _menuRepository.DeleteMenuAsync(Id, TableName);
        }

        public List<MenuStructureDto> GetMenuHierarchy()
        {
            return _menuRepository.GetMenuHierarchy();
        }
    }
}
