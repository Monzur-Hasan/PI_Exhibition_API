using Application.Db_Helper;
using Dapper;
using Domain.Models.Permission.DTO;
using Domain.Models.Permission.Filter;
using Domain.OtherModels.Response;
using Infrastructure.Features.Repository.Administrator;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Application.Features.Repository.Administrator
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DbHelper _dbHelper;

        public UserPermissionRepository(IDbConnection dbConnection, DbHelper dbHelper)
        {
            _dbConnection = dbConnection;
            _dbHelper = dbHelper;
        }

        public async Task<IEnumerable<GetPermissionDto>> GetActivePermissionsAsync(UserPermission_Filter userPermission_Filter)
        {
            try
            {
                var roleIdQuery = @"Select Id from AspNetRoles where RegistrationType = @RegistrationType";
                var roleId = await _dbConnection.QueryFirstOrDefaultAsync<Guid>(roleIdQuery, new { RegistrationType = "Employee" });


                string sqlQuery = @"
                SELECT 
                    p.PermissionID,
                    p.PermissionKey,
                    p.PermissionName,
                    p.IsActive,
                    p.StateStatus,
                    p.IsApproved,
                    b.AgencyName,
                    IsAllowed=ISNULL(pu.IsAllowed, 0)
                FROM 
                    tblPermission p
                INNER JOIN 
                    tblAgency b ON p.AgencyID = b.AgencyID
                LEFT JOIN 
                    tblUserPermission pu ON p.PermissionID = pu.PermissionID 
                    AND pu.AgencyEmpID = @AgencyEmpID
                INNER JOIN
                    tblRolePermission RP ON RP.PermissionID = p.PermissionID
                WHERE 
                    p.IsActive = 1 
                    AND b.IsActive = 1
                    AND (@PermissionID IS NULL OR @PermissionID = '' OR @PermissionID = '0' OR p.PermissionID = @PermissionID)
                    AND (@StateStatus IS NULL OR @StateStatus = '' OR @StateStatus = '0' OR p.StateStatus = @StateStatus)
                    AND (@IsActive IS NULL OR @IsActive = 'true' OR p.IsActive = @IsActive)
                    AND (@IsApproved IS NULL OR @IsApproved = 'false' OR @IsApproved = 'true' OR p.IsApproved = @IsApproved)
                    AND (@AgencyID IS NULL OR @AgencyID = '' OR @AgencyID = '0' OR p.AgencyID = @AgencyID)
                    AND RP.RoleID = @RoleID
                ORDER BY 
                    p.PermissionID;";

                var parameters = new DynamicParameters();
                parameters.Add("AgencyEmpID", userPermission_Filter.AgencyEmpID);
                parameters.Add("PermissionID", userPermission_Filter.PermissionID);
                parameters.Add("AgencyID", userPermission_Filter.AgencyID);
                parameters.Add("IsActive", userPermission_Filter.IsActive);
                parameters.Add("IsApproved", userPermission_Filter.IsApproved);
                parameters.Add("StateStatus", userPermission_Filter.StateStatus);
                parameters.Add("RoleID", roleId);

                var data = await _dbConnection.QueryAsync<GetPermissionDto>(sqlQuery, parameters);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching permissions: {ex.Message}", ex);
            }
        }

        public async Task<ExecutionStatus> SaveMenuPermissionAsync(UserPermissionRequest request)
        {
            Thread.Sleep(20);
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var rowCount = 0;
               // var allowedPermissions = request.Permissions.Where(p => p.IsAllowed).ToList();

                foreach (var permission in request.Permissions)
                {                   
                    var checkSql = @"
                SELECT COUNT(1) 
                FROM tblUserPermission 
                WHERE AgencyEmpID = @AgencyEmpID 
                  AND AgencyID = @AgencyID 
                  AND PermissionID = @PermissionID";

                    var exists = await _dbConnection.ExecuteScalarAsync<int>(checkSql, new
                    {
                        request.AgencyEmpID,
                        request.AgencyID,
                        permission.PermissionID
                    }, transaction);

                    if (exists > 0)
                    {                       
                        var updateSql = @"
                    UPDATE tblUserPermission
                    SET 
                        IsAllowed = @IsAllowed,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = GETDATE(),
                        IsActive = @IsActive,
                        ActivationStatus = @ActivationStatus,
                        IsApproved = @IsApproved,
                        StateStatus = @StateStatus, UserID = @UserID
                    WHERE 
                        AgencyEmpID = @AgencyEmpID 
                        AND AgencyID = @AgencyID 
                        AND PermissionID = @PermissionID";

                        rowCount += await _dbConnection.ExecuteAsync(updateSql, new
                        {
                            permission.PermissionID,
                            permission.IsAllowed,
                            UpdatedBy = request.AgencyID,
                            permission.IsActive,
                            permission.ActivationStatus,
                            permission.IsApproved,
                            permission.StateStatus,
                            request.AgencyEmpID,
                            request.UserID,
                            request.AgencyID
                        }, transaction);
                    }
                    else
                    {                     
                        var user_permission_id = await _dbHelper.GetProductCodeAsync("UserPermissionID", "tblUserPermission", (SqlTransaction)transaction);

                        if (!string.IsNullOrEmpty(user_permission_id))
                        {
                            var insertSql = @"
                        INSERT INTO tblUserPermission (
                            UserPermissionID,
                            PermissionID,
                            AgencyEmpID,
                            AgencyID,
                            IsAllowed,
                            CreatedBy,
                            CreatedDate,
                            IsActive,
                            ActivationStatus,
                            IsApproved,
                            StateStatus, UserID)
                        VALUES (
                            @UserPermissionID,
                            @PermissionID,
                            @AgencyEmpID,
                            @AgencyID,
                            @IsAllowed,
                            @CreatedBy,
                            GETDATE(),
                            @IsActive,
                            @ActivationStatus,
                            @IsApproved,
                            @StateStatus, @UserID)";

                            rowCount += await _dbConnection.ExecuteAsync(insertSql, new
                            {
                                UserPermissionID = user_permission_id,
                                permission.PermissionID,
                                request.AgencyEmpID,
                                request.UserID,
                                request.AgencyID,
                                permission.IsAllowed,
                                CreatedBy = request.AgencyID,
                                permission.IsActive,
                                permission.ActivationStatus,
                                permission.IsApproved,
                                permission.StateStatus
                            }, transaction);
                        }
                    }
                }

                if (rowCount > 0)
                {
                    transaction.Commit();
                    return new ExecutionStatus { Status = true, Msg = "Permissions saved successfully", StatusCode = "200" };
                }

                transaction.Rollback();
                return new ExecutionStatus { Status = false, Msg = "No permissions were saved", StatusCode = "400" };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ExecutionStatus { Status = false, Msg = ex.Message, StatusCode = "500" };
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<GetPermissionDto>> GetAppUserMenusAsync(string? userName)
        {
            try
            {
                string sqlQuery = @"
                SELECT p.AgencyID, p.MainMenuID, p.MainMenuName, p.SubMenuID, p.PermissionType,
                p.PermissionID, p.PermissionKey, p.PermissionName, p.IsActive, p.StateStatus, p.IsApproved, b.AgencyName,IsAllowed=pu.IsAllowed, u.RegistrationType
                FROM 
                tblPermission p
                INNER JOIN 
                tblAgency b ON p.AgencyID = b.AgencyID
                INNER JOIN 
                tblUserPermission pu ON p.PermissionID = pu.PermissionID 
                AND pu.AgencyID = p.AgencyID AND pu.IsAllowed=1
                INNER JOIN AspNetUsers u ON pu.AgencyEmpID = u.EmployeeId --AND pu.UserID = U.Id
               WHERE 1=1 AND u.RegistrationType = 'Employee' AND
                p.IsActive = 1 AND b.IsActive = 1 AND U.IsActive=1 AND u.UserName = @UserName --AND u.Id = @UserID
                ORDER BY 
                p.SubMenuID, p.MainMenuID;";

                var parameters = new DynamicParameters();
                parameters.Add("UserName", userName);


                var data = await _dbConnection.QueryAsync<GetPermissionDto>(sqlQuery, parameters);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching permissions: {ex.Message}", ex);
            }
        }
    }
}
