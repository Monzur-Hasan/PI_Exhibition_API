using System.Data;
using Application.Db_Helper;
using Dapper;
using Domain.Models.Access.DTO;
using Domain.Models.Administrator.DomainModel;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;
using Domain.Shared.Configurations;
using Infrastructure.Features.Repository.Administrator;
using Microsoft.Data.SqlClient;


namespace Application.Features.Repository.Administrator
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DbHelper _dbHelper;

        public MenuRepository(IDbConnection dbConnection, DbHelper dbHelper)
        {
            _dbConnection = dbConnection;
            _dbHelper = dbHelper;
        }

        //Main menu Repository
        public async Task<bool> InsertMainMenuAsync(MainMenuInputDto menu)
        {
            Thread.Sleep(20);
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var MMId = await _dbHelper.GetProductCodeAsync("MMId", "tblMainMenus", (SqlTransaction)transaction);
                MMId = "MM" + MMId;
                var query = @"
                INSERT INTO tblMainMenus (MMId, MenuName, ShortName, IconClass, IconColor, MId, CreatedBy, CreatedDate, ApplicationId, IsActive, SequenceNo, ServiceID)
                VALUES (@MMId, @MenuName, @ShortName, @IconClass, @IconColor, @MId, @CreatedBy, @CreatedDate, @ApplicationId, @IsActive, @SequenceNo, @ServiceID);
                ";

                var result = await _dbConnection.ExecuteAsync(query, new
                {
                    MMId,
                    menu.MenuName,
                    menu.ShortName,
                    menu.IconClass,
                    menu.IconColor,
                    menu.MId,
                    menu.CreatedBy,
                    CreatedDate = DateTime.Now,
                    menu.ApplicationId,
                    menu.IsActive,
                    menu.SequenceNo,
                    menu.ServiceID,
                }, transaction);
                if (result < 1)
                {
                    transaction.Rollback();
                    return false;
                }
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error (use a logging framework)
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsMainMenuExistsAsync(string ShortName, string MenuName)
        {
            try
            {
                var query = @"
                SELECT COUNT(*) 
                FROM tblMainMenus 
                WHERE MenuName = @MenuName OR ShortName = @ShortName";

                int count = await _dbConnection.ExecuteScalarAsync<int>(
                    query,
                    new
                    {
                        MenuName,
                        ShortName
                    });
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsMainMenuExistsForUpdateAsync(string MenuName, string MMId)
        {
            try
            {
                var query = @"
                    SELECT COUNT(1) 
                    FROM tblMainMenus 
                    WHERE MenuName = @MenuName 
                    AND MMId != @MMId";

                int count = await _dbConnection.ExecuteScalarAsync<int>(
                    query,
                    new
                    {
                        MenuName,
                        MMId
                    });
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<DBResponse<MainMenu>> GetMainMenusAsync(MainMenuFilter filter)
        {
            DBResponse<MainMenu> data = new DBResponse<MainMenu>();

            try
            {
                string sqlQuery = @"
                WITH Data_CTE AS (
                    SELECT *
                    FROM tblMainMenus
                    WHERE (@MenuName IS NULL OR @MenuName = '' OR MenuName = @MenuName)
                ),
                Count_CTE AS (
                    SELECT COUNT(*) AS [TotalRows] FROM Data_CTE
                )
                SELECT 
                    JSONData = (
                        SELECT * 
                        FROM (SELECT * FROM Data_CTE 
                              ORDER BY MMId DESC 
                              OFFSET (@PageNumber - 1) * @PageSize ROWS 
                              FETCH NEXT CAST(@PageSize AS INT) ROWS ONLY) tbl 
                        FOR JSON AUTO
                    ),
                    TotalRows = (SELECT TotalRows FROM Count_CTE),
                    PageSize = @PageSize,
                    PageNumber = @PageNumber;
                    ";

                

                var parameters = new DynamicParameters();
                parameters.Add("MenuName", filter.MenuName);
                parameters.Add("PageNumber", filter.PageNumber);
                parameters.Add("PageSize", filter.PageSize);

                var response = await _dbConnection.QueryFirstOrDefaultAsync<DBResponse>(sqlQuery, parameters);

                if (response != null)
                {
                    data.ListOfObject = JsonReverseConverter.JsonToObject<IEnumerable<MainMenu>>(response.JSONData) ?? new List<MainMenu>();
                    data.Pageparam = new Pageparam
                    {
                        PageNumber = response.PageNumber,
                        PageSize = response.PageSize,
                        TotalPages = (int)Math.Ceiling((double)response.TotalRows / filter.PageSize),
                        TotalRows = response.TotalRows
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Main Menu.", ex);
            }

            return data;
        }

        public async Task<MainMenu> GetMainMenuByIdAync(string MMId)
        {
            try
            {
                var query = @"
                SELECT * 
                FROM tblMainMenus 
                WHERE MMId = @MMId";
                var result = await _dbConnection.QueryFirstOrDefaultAsync<MainMenu>(query, new { MMId });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ExecutionStatus> UpdateMainMenuTypeAsync(string SubmenuId, Dictionary<string, object> changedFields)
        {
            var executionStatus = new ExecutionStatus();
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                // Step 2: Update tblComplains
                var setClauses = changedFields.Select(f => $"{f.Key} = @{f.Key}");
                string updateMainMenuQuery = $@"
                UPDATE tblSubMenus
                SET {string.Join(", ", setClauses)}
                WHERE SubmenuId = @SubmenuId";

                var parameters = new DynamicParameters(changedFields);
                parameters.Add("SubmenuId", SubmenuId);

                int affectedRows = await _dbConnection.ExecuteAsync(updateMainMenuQuery, parameters, transaction);

                if (affectedRows <= 0)
                {
                    executionStatus.Status = false;
                    executionStatus.Msg = "No changes made.";
                    executionStatus.StatusCode = "400";
                    transaction.Rollback();
                    return executionStatus;
                }

                transaction.Commit();
                executionStatus.Status = true;
                executionStatus.Msg = "Main menu updated successfully.";
                executionStatus.StatusCode = "200";
                executionStatus.Code = SubmenuId;
                return executionStatus;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                executionStatus.Status = false;
                executionStatus.ErrorMsg = ex.Message;
                executionStatus.StatusCode = "500";
                return executionStatus;
            }
            finally
            {
                _dbConnection.Close();
            }
        }


        //Sub menu Repository
        public async Task<bool> InsertSubMenuAsync(SubmenuDto submenu)
        {
            Thread.Sleep(20);
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var SMId = await _dbHelper.GetProductCodeAsync("SubmenuId", "tblSubMenus", (SqlTransaction)transaction);
                SMId = "SM" + SMId;
                var query = @"INSERT INTO tblSubMenus (
                    SubmenuId,
                    SubmenuName,
                    ControllerName,
                    ActionName,
                    Path,
                    Component,
                    IconClass,
                    IconColor,
                    IsViewable,
                    IsActAsParent,
                    HasTab,
                    ParentSubmenuId,
                    IsActive,
                    MMId,
                    ModuleId,
                    ApplicationId,
                    CreatedBy,
                    CreatedDate,
                    MenuSequence
                ) VALUES (
                    @SubmenuId,
                    @SubmenuName,
                    @ControllerName,
                    @ActionName,
                    @Path,
                    @Component,
                    @IconClass,
                    @IconColor,
                    @IsViewable,
                    @IsActAsParent,
                    @HasTab,
                    @ParentSubmenuId,
                    @IsActive,
                    @MMId,
                    @ModuleId,
                    @ApplicationId,
                    @CreatedBy,
                    @CreatedDate,
                    @MenuSequence
                );";                        


                var result = await _dbConnection.ExecuteAsync(query, new
                {
                    SubmenuId = SMId,
                    submenu.SubmenuName,
                    submenu.ControllerName,
                    submenu.ActionName,
                    submenu.Path,
                    submenu.Component,
                    submenu.IconClass,
                    submenu.IconColor,
                    submenu.IsViewable,
                    submenu.IsActAsParent,
                    submenu.HasTab,
                    submenu.ParentSubmenuId,
                    IsActive = true,
                    submenu.MMId,
                    submenu.ModuleId,
                    submenu.ApplicationId,
                    submenu.CreatedBy,
                    CreatedDate = DateTime.Now,
                    submenu.MenuSequence,
                }, transaction);

                if (result < 1)
                {
                    transaction.Rollback();
                    return false;
                }
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error (use a logging framework)
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsSubMenuExistsAsync(string SubmenuName, string Path)
        {
            try
            {
                var query = "";
                int count = 0;
                if (Path != "N/A")
                {
                    query = @"
                    SELECT COUNT(*) 
                    FROM tblSubMenus 
                    WHERE SubmenuName = @SubmenuName OR Path = @Path";
                    count = await _dbConnection.ExecuteScalarAsync<int>(
                    query,
                    new
                    {
                        SubmenuName,
                        Path
                    });
                }
                else
                {
                    query = @"
                    SELECT COUNT(*) 
                    FROM tblSubMenus 
                    WHERE SubmenuName = @SubmenuName";
                    count = await _dbConnection.ExecuteScalarAsync<int>(
                    query,
                    new
                    {
                        SubmenuName,
                    });
                }

                
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<SubMenu> GetSubMenuByIdAync(string SubmenuId)
        {
            try
            {
                var query = @"
                SELECT * 
                FROM tblSubMenus 
                WHERE SubmenuId = @SubmenuId";
                var result = await _dbConnection.QueryFirstOrDefaultAsync<SubMenu>(query, new { SubmenuId });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<DBResponse<SubMenu>> GetSubMenusAsync(SubMenuFilter filter)
        {
            DBResponse<SubMenu> data = new DBResponse<SubMenu>();

            try
            {
                string sqlQuery = @"
                WITH Data_CTE AS (
                    SELECT *
                    FROM tblSubMenus
                    WHERE (@SubmenuName IS NULL OR @SubmenuName = '' OR SubmenuName = @SubmenuName)
                ),
                Count_CTE AS (
                    SELECT COUNT(*) AS [TotalRows] FROM Data_CTE
                )
                SELECT 
                    JSONData = (
                        SELECT * 
                        FROM (SELECT * FROM Data_CTE 
                              ORDER BY SubmenuId DESC 
                              OFFSET (@PageNumber - 1) * @PageSize ROWS 
                              FETCH NEXT CAST(@PageSize AS INT) ROWS ONLY) tbl 
                        FOR JSON AUTO
                    ),
                    TotalRows = (SELECT TotalRows FROM Count_CTE),
                    PageSize = @PageSize,
                    PageNumber = @PageNumber;
                    ";



                var parameters = new DynamicParameters();
                parameters.Add("SubmenuName", filter.SubmenuName);
                parameters.Add("PageNumber", filter.PageNumber);
                parameters.Add("PageSize", filter.PageSize);

                var response = await _dbConnection.QueryFirstOrDefaultAsync<DBResponse>(sqlQuery, parameters);

                if (response != null)
                {
                    data.ListOfObject = JsonReverseConverter.JsonToObject<IEnumerable<SubMenu>>(response.JSONData) ?? new List<SubMenu>();
                    data.Pageparam = new Pageparam
                    {
                        PageNumber = response.PageNumber,
                        PageSize = response.PageSize,
                        TotalPages = (int)Math.Ceiling((double)response.TotalRows / filter.PageSize),
                        TotalRows = response.TotalRows
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Sub Menu.", ex);
            }

            return data;
        }

        public async Task<ExecutionStatus> UpdateSubMenuTypeAsync(string SubmenuId, Dictionary<string, object> changedFields)
        {
            var executionStatus = new ExecutionStatus();
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                // Step 2: Update tblComplains
                var setClauses = changedFields.Select(f => $"{f.Key} = @{f.Key}");
                string updateSubMenuQuery = $@"
                UPDATE tblSubMenus
                SET {string.Join(", ", setClauses)}
                WHERE SubmenuId = @SubmenuId";

                var parameters = new DynamicParameters(changedFields);
                parameters.Add("SubmenuId", SubmenuId);

                int affectedRows = await _dbConnection.ExecuteAsync(updateSubMenuQuery, parameters, transaction);

                if (affectedRows <= 0)
                {
                    executionStatus.Status = false;
                    executionStatus.Msg = "No changes made.";
                    executionStatus.StatusCode = "400";
                    transaction.Rollback();
                    return executionStatus;
                }

                transaction.Commit();
                executionStatus.Status = true;
                executionStatus.Msg = "Main menu updated successfully.";
                executionStatus.StatusCode = "200";
                executionStatus.Code = SubmenuId;
                return executionStatus;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                executionStatus.Status = false;
                executionStatus.ErrorMsg = ex.Message;
                executionStatus.StatusCode = "500";
                return executionStatus;
            }
            finally
            {
                _dbConnection.Close();
            }
        }


        //Shared Repository
        public async Task<ExecutionStatus> DeleteMenuAsync(string Id, string TableName)
        {
            var executionStatus = new ExecutionStatus();
            int dtlRowCount = 0;
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                if (TableName == "tblMainMenus")
                {
                    dtlRowCount = await _dbConnection.ExecuteAsync(
                        "DELETE FROM tblMainMenus WHERE MMId = @MMId",
                        new { MMId = Id }, transaction);
                }
                else if (TableName == "tblSubMenus")
                {
                    dtlRowCount = await _dbConnection.ExecuteAsync(
                        "DELETE FROM tblSubMenus WHERE SubmenuId = @SubmenuId",
                        new { SubmenuId = Id }, transaction);
                }
                else
                {
                    transaction.Rollback();
                    executionStatus.Status = false;
                    executionStatus.Msg = "Invalid table name.";
                    executionStatus.StatusCode = "400";
                    return executionStatus;
                }

                if (dtlRowCount > 0)
                {
                    transaction.Commit();
                    executionStatus.Status = true;
                    executionStatus.Msg = "Menu deleted successfully.";
                    executionStatus.StatusCode = "200";
                    return executionStatus;
                }
                else
                {
                    transaction.Rollback();
                    executionStatus.Status = false;
                    executionStatus.Msg = "Menu delete failed.";
                    executionStatus.StatusCode = "304";
                    return executionStatus;
                }

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                executionStatus.Status = false;
                executionStatus.ErrorMsg = ex.Message;
                executionStatus.StatusCode = "500";
                return executionStatus;
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        private void BuildSubMenuHierarchy(SubmenuStructureDto parentSubMenu, List<SubmenuStructureDto> allSubMenus)
        {
            if (parentSubMenu.IsActAsParent)
            {
                parentSubMenu.ChildSubMenus = allSubMenus
                    .Where(sm => sm.ParentSubmenuId == parentSubMenu.SubmenuId)
                    .OrderBy(sm => sm.MenuSequence)
                    .ToList();

                foreach (var childSubMenu in parentSubMenu.ChildSubMenus)
                {
                    BuildSubMenuHierarchy(childSubMenu, allSubMenus);
                }
            }
        }
        public List<MenuStructureDto> GetMenuHierarchy()
        {
            // Fetch all active main menus
            var mainMenus = _dbConnection.Query<MenuStructureDto>(
                "SELECT MMId, MenuName FROM tblMainMenus WHERE IsActive = 1"
            ).ToList();

            // Fetch all active submenus
            var subMenus = _dbConnection.Query<SubmenuStructureDto>(
                "SELECT SubmenuId, SubmenuName, IsActAsParent, ParentSubmenuId, MMId, MenuSequence " +
                "FROM tblSubMenus WHERE IsActive = 1"
            ).ToList();
            // Build the hierarchy
            foreach (var mainMenu in mainMenus)
            {
                mainMenu.SubMenus = subMenus
                    .Where(sm => sm.MMId == mainMenu.MMId && sm.ParentSubmenuId == "N/A")
                    .OrderBy(sm => sm.MenuSequence)
                    .ToList();

                foreach (var subMenu in mainMenu.SubMenus)
                {
                    BuildSubMenuHierarchy(subMenu, subMenus);
                }
            }

            return mainMenus;
        }
    }
}
