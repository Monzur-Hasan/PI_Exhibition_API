using Application.Db_Helper;
using Dapper;
using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Filter;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;
using Domain.Shared.Configurations;
using Infrastructure.Features.Repository.Administrator;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Application.Features.Repository.Administrator
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DbHelper _dbHelper;

        public RoleRepository(IDbConnection dbConnection, DbHelper dbHelper)
        {
            _dbConnection = dbConnection;
            _dbHelper = dbHelper;
        }
        public async Task<DBResponse<GetApplicationRoleDto>> GetAllRolesAsync(ApplicationRole_Filter filter)
        {
            DBResponse<GetApplicationRoleDto> data = new();

            try
            {
                var search_cond = "";
                var search_string = "";
                //if (filter.SearchString != null && filter.SearchString != "")
                //{
                //    search_cond += $@" AND (CONCAT(a.Name, a.ComplainName, a.[Description], a.CurrentComments, i.[LandlordID], a.[AgencyID], a.[PropertyID], p.[PropertyName], p.[PropertyType], b.LandlordName, b.LandlordType, c.AgencyName, a.StateStatus, a.Feedback, a.TicketNo,
                //     pws.SegmentID, pws.SegmentName, pws.SegmentLength, pws.SegmentWidth, pws.SegmentDescription
                //    ) LIKE N'%{filter.SearchString}%')";
                //}
                var query = $@"
                WITH Data_CTE AS (
                 SELECT a.[Id], a.[Name], a.[RegistrationType], a.[NormalizedName], a.[ConcurrencyStamp], a.[IsActive], a.[Description], a.[OrganizationId], a.[CreatedBy], a.[CreatedDate], a.[UpdatedBy], a.[UpdatedDate], a.[CompanyId], a.[IsBranchAdmin], a.[IsCompanyAdmin], a.[IsGroupAdmin], a.[IsSysadmin], a.[DeletedBy], a.[DeletedDate], a.[BranchId]
                FROM AspNetRoles a  
                WHERE  1=1 and a.IsActive=1	
		        AND (@IsActive IS NULL OR @IsActive = 'true'  OR @IsActive = 'true' OR a.IsActive = @IsActive)
                ),
                Count_CTE AS (
                SELECT COUNT(*) AS [TotalRows] FROM Data_CTE
                )
                SELECT 
                JSONData = (
                    SELECT * 
                    FROM (SELECT * FROM Data_CTE 
                            ORDER BY Id
                            OFFSET (@PageNumber - 1) * @PageSize ROWS 
                            FETCH NEXT CAST(@PageSize AS INT) ROWS ONLY) tbl 
                    FOR JSON AUTO
                ),
                TotalRows = (SELECT TotalRows FROM Count_CTE),
                TotalPages = CEILING(1.0 * (SELECT TotalRows FROM Count_CTE) / @PageSize),
                PageSize = @PageSize,
                PageNumber = @PageNumber;
                    ";

                var parameters = new DynamicParameters();
                parameters.Add("IsActive", filter.IsActive);
                parameters.Add("PageNumber", filter.PageNumber);
                parameters.Add("PageSize", filter.PageSize);

                var response = await _dbConnection.QueryFirstOrDefaultAsync<DBResponse>(query, parameters);

                if (response != null)
                {
                    data.ListOfObject = JsonReverseConverter.JsonToObject<IEnumerable<GetApplicationRoleDto>>(response.JSONData) ??
                      new List<GetApplicationRoleDto>();
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
                Console.WriteLine(ex);
            }
            return data;
        }

        public async Task<IEnumerable<GetRolePermissionDto>> GetActivePermissionsAsync(ActivePermissions_Filter filter)
        {
            try
            {
                string sqlQuery = @"          
                SELECT 
                  p.PermissionID,
                  p.PermissionKey,
                  p.PermissionName,
                  p.IsActive,
                  p.StateStatus,
                  p.IsApproved,
                  IsAllowed=ISNULL(rp.IsAllowed, 0)
                  FROM 
                  tblPermission p
                  LEFT JOIN 
                  tblRolePermission rp ON p.PermissionID = rp.PermissionID 
                  AND rp.RoleID = @RoleID
                  WHERE 
                  p.IsActive = 1 
                  AND (@PermissionID IS NULL OR @PermissionID = '' OR @PermissionID = '0' OR p.PermissionID = @PermissionID)
                  AND (@StateStatus IS NULL OR @StateStatus = '' OR @StateStatus = '0' OR p.StateStatus = @StateStatus)
                  AND (@IsActive IS NULL OR @IsActive = 'true' OR p.IsActive = @IsActive)
                  AND (@IsApproved IS NULL OR @IsApproved = 'false' OR @IsApproved = 'true' OR p.IsApproved = @IsApproved)
                  AND (@AgencyID IS NULL OR @AgencyID = '' OR @AgencyID = '0' OR p.AgencyID = @AgencyID)
                  ORDER BY 
                  p.PermissionID;";

                var parameters = new DynamicParameters();
                parameters.Add("RoleID", filter.RoleID);
                parameters.Add("PermissionID", filter.PermissionID);
                parameters.Add("AgencyID", filter.AgencyID);
                parameters.Add("IsActive", filter.IsActive);
                parameters.Add("IsApproved", filter.IsApproved);
                parameters.Add("StateStatus", filter.StateStatus);

                var data = await _dbConnection.QueryAsync<GetRolePermissionDto>(sqlQuery, parameters);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching permissions: {ex.Message}", ex);
            }
        }

        public async Task<ExecutionResponse<CreateRoleResultDto>> CreateRoleAsync(CreateRoleRequestDto createRoleDto)
        {
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                string query = $@"SELECT Id FROM AspNetRoles WHERE Name LIKE @NamePattern";
                var existingRoleId = await _dbConnection.QueryFirstOrDefaultAsync<Guid?>(
                   query,
                    new { NamePattern = $"%{createRoleDto.Name}%" },
                    transaction
                );

                Guid roleId;

                if (existingRoleId.HasValue)
                {
                    roleId = existingRoleId.Value;
                }
                else
                {
                    roleId = Guid.NewGuid();

                    var roleInsertQuery = $@"
                    INSERT INTO AspNetRoles(Id, [Name], [RegistrationType], [NormalizedName], [ConcurrencyStamp], [IsActive], [Description], [CreatedBy], [CreatedDate])
                    VALUES (@Id, @Name, @RegistrationType, @NormalizedName, @ConcurrencyStamp, @IsActive, @Description, @CreatedBy, @CreatedDate)";

                    await _dbConnection.ExecuteAsync(roleInsertQuery, new
                    {
                        Id = roleId,
                        createRoleDto.Name,
                        RegistrationType = createRoleDto.Name,
                        createRoleDto.IsActive,
                        createRoleDto.Description,
                        CreatedBy = "Admin",
                        CreatedDate = DateTime.Now,
                        NormalizedName = createRoleDto.Name.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid()
                    }, transaction);
                }

                int rowCount2 = 0;

                if (createRoleDto.Permissions != null && createRoleDto.Permissions.Any())
                {
                    foreach (var permission in createRoleDto.Permissions)
                    {

                        var existingPermission = await _dbConnection.QueryFirstOrDefaultAsync<int>(
                            $@"SELECT COUNT(1) FROM tblRolePermission WHERE RoleID = @RoleID AND PermissionID = @PermissionID",
                            new { RoleID = roleId, PermissionID = permission },
                            transaction
                        );

                        if (existingPermission > 0)
                        {

                            var updateQuery = @"
                            UPDATE tblRolePermission
                            SET IsAllowed = @IsAllowed,
                                IsActive = @IsActive,
                                StateStatus = @StateStatus,
                                UpdatedBy = @UpdatedBy,
                                UpdatedDate = @UpdatedDate
                            WHERE RoleID = @RoleID AND PermissionID = @PermissionID";

                            rowCount2 += await _dbConnection.ExecuteAsync(updateQuery, new
                            {
                                RoleID = roleId,
                                PermissionID = permission,
                                IsAllowed = true,
                                IsActive = true,
                                StateStatus = "Approved",
                                UpdatedBy = "Admin",
                                UpdatedDate = DateTime.Now
                            }, transaction);
                        }
                        else
                        {

                            var rolePermissionInsertQuery = @"
                            INSERT INTO tblRolePermission(RolePermissionID, RoleID, PermissionID, IsAllowed, IsActive, StateStatus, CreatedBy, CreatedDate, ActivationStatus)
                            VALUES (@RolePermissionID, @RoleID, @PermissionID, @IsAllowed, @IsActive, @StateStatus, @CreatedBy, @CreatedDate, @ActivationStatus)";

                            var rolePermissionId = await _dbHelper.GetProductCodeAsync("RolePermissionID", "tblRolePermission", (SqlTransaction)transaction);

                            rowCount2 += await _dbConnection.ExecuteAsync(rolePermissionInsertQuery, new
                            {
                                RolePermissionID = rolePermissionId,
                                RoleID = roleId,
                                PermissionID = permission,
                                IsAllowed = true,
                                IsActive = true,
                                StateStatus = "Approved",
                                CreatedBy = "Admin",
                                CreatedDate = DateTime.Now,
                                ActivationStatus = "Active"
                            }, transaction);
                        }
                    }
                }

                if (rowCount2 > 0)
                {
                    transaction.Commit();
                    return ExecutionResponse<CreateRoleResultDto>.SuccessResponse(
                        message: $"Role '{createRoleDto.Name}' created successfully.",
                        data: new CreateRoleResultDto
                        {
                            Id = roleId.ToString(),
                            Name = createRoleDto.Name,
                            IsActive = createRoleDto.IsActive,
                            Description = createRoleDto.Description,
                        }
                    );
                }
                else
                {
                    transaction.Rollback();
                }

                return ExecutionResponse<CreateRoleResultDto>.FailureResponse(
                    statusCode: 400,
                    message: $"Error processing role '{createRoleDto.Name}'"
                );
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ExecutionResponse<CreateRoleResultDto>.FailureResponse(
                    statusCode: 400,
                    message: $"Error processing role '{createRoleDto.Name}': {ex.Message}"
                );
            }
            finally
            {
                _dbConnection.Close();
            }
        }


        public async Task<ExecutionResponse<List<string>>> GetRolePermissionsAsync(string[] roleIds)
        {
            try
            {
                var validRoleGuids = new List<Guid>();
                foreach (var id in roleIds)
                {
                    if (Guid.TryParse(id, out Guid roleGuid))
                    {
                        validRoleGuids.Add(roleGuid);
                    }
                    else
                    {
                        return ExecutionResponse<List<string>>.FailureResponse(
                            message: $"Invalid role ID format in '{id}'",
                            statusCode: 400);
                    }
                }

                if (!validRoleGuids.Any())
                {
                    return ExecutionResponse<List<string>>.FailureResponse(
                        message: "No valid role IDs provided",
                        statusCode: 400);
                }

                string sql = @"
                SELECT DISTINCT PermissionID 
                FROM tblRolePermission 
                WHERE RoleID IN @RoleIds AND IsAllowed = 1";

                var permissions = await _dbConnection.QueryAsync<string>(sql, new { RoleIds = validRoleGuids });

                if (!permissions.Any())
                {
                    return ExecutionResponse<List<string>>.FailureResponse(
                        message: "No permissions found for the provided role IDs",
                        statusCode: 404);
                }

                return ExecutionResponse<List<string>>.SuccessResponse(
                    message: "Permissions retrieved successfully",
                    data: permissions.ToList());
            }
            catch (Exception ex)
            {
                var statusCode = 400;

                return ExecutionResponse<List<string>>.FailureResponse(
                    message: $"Error retrieving permissions: {ex.Message}",
                    statusCode: statusCode
                   );
            }
        }

        public async Task<ExecutionResponse<string>> UpdateRoleAsync(UpdateRoleRequestDto updateRoleDto)
        {
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var existingRole = await _dbConnection.QueryFirstOrDefaultAsync<Guid?>(
                    "SELECT Id FROM AspNetRoles WHERE 1=1 AND Id = @Id",
                    new { updateRoleDto.Id },
                    transaction
                );

                if (existingRole == null)
                {
                    return ExecutionResponse<string>.FailureResponse(
                        message: $"Role with ID '{updateRoleDto.Id}' not found.",
                        statusCode: 404
                    );
                }

                var updateRoleQuery = @"
                UPDATE AspNetRoles 
                SET Name = @Name, 
                    Description = @Description, 
                    IsActive = @IsActive, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedDate = @UpdatedDate
                WHERE 1=1 AND Id = @Id";

                int updatedRows = await _dbConnection.ExecuteAsync(updateRoleQuery, new
                {
                    updateRoleDto.Id,
                    updateRoleDto.Name,
                    updateRoleDto.Description,
                    updateRoleDto.IsActive,
                    updateRoleDto.UpdatedBy,
                    UpdatedDate = DateTime.UtcNow
                }, transaction);

                int rowCount = 0;
                if (updateRoleDto.Permissions != null && updateRoleDto.Permissions.Any())
                {
                    var deletePermissionsQuery = "DELETE FROM tblRolePermission WHERE 1=1 AND RoleId = @RoleId";
                    await _dbConnection.ExecuteAsync(deletePermissionsQuery, new { RoleId = updateRoleDto.Id }, transaction);

                    var insertPermissionsQuery = @"
                    INSERT INTO tblRolePermission (RolePermissionID, RoleID, PermissionID, AgencyID, IsAllowed, IsActive, StateStatus, CreatedBy, CreatedDate, ActivationStatus)
                    VALUES (@RolePermissionID, @RoleID, @PermissionID, @AgencyID, @IsAllowed, @IsActive, @StateStatus, @CreatedBy, @CreatedDate, @ActivationStatus);";

                    foreach (var permission in updateRoleDto.Permissions)
                    {
                        var rolePermissionId = await _dbHelper.GetProductCodeAsync("RolePermissionID", "tblRolePermission", (SqlTransaction)transaction);
                        var rolePermission = new
                        {
                            RolePermissionID = rolePermissionId,
                            RoleID = updateRoleDto.Id,
                            PermissionID = permission,
                            updateRoleDto.AgencyID,
                            IsAllowed = true,
                            IsActive = true,
                            StateStatus = "Approved",
                            CreatedBy = updateRoleDto.UpdatedBy,
                            CreatedDate = DateTime.UtcNow,
                            ActivationStatus = "Active"
                        };

                        rowCount += await _dbConnection.ExecuteAsync(insertPermissionsQuery, rolePermission, transaction);
                    }
                }
                if (rowCount > 0)
                {
                    transaction.Commit();
                    return ExecutionResponse<string>.SuccessResponse(
                        message: $"Role '{updateRoleDto.Name}' updated successfully.",
                        data: updateRoleDto.Id
                    );
                }
                else
                {
                    transaction.Rollback();
                }              

                return ExecutionResponse<string>.FailureResponse(
                    statusCode: 400,
                    message: $"Role '{updateRoleDto.Name}' updated failed."                    
                );
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ExecutionResponse<string>.FailureResponse(
                    message: $"Error updating role '{updateRoleDto.Id}': {ex.Message}",
                    statusCode: 500
                );
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<ExecutionResponse<string>> DeleteRoleAsync(DeleteRoleRequestDto deleteRoleRequestDto)
        {
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            try
            {               
                var existingRole = await _dbConnection.QueryFirstOrDefaultAsync<Guid?>(
                    "SELECT Id FROM AspNetRoles WHERE Id = @Id",
                    new { deleteRoleRequestDto.Id },
                    transaction
                );

                if (existingRole == null)
                {
                    return ExecutionResponse<string>.FailureResponse(
                        message: $"Role with Name '{deleteRoleRequestDto.Name}' not found.",
                        statusCode: 404
                    );
                }
               
                var userCount = await _dbConnection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM AspNetUserRoles WHERE RoleId = @RoleId",
                    new { RoleId = existingRole },
                    transaction
                );

                if (userCount > 0)
                {
                    return ExecutionResponse<string>.FailureResponse(
                        message: $"Role '{deleteRoleRequestDto.Id}' cannot be deleted because it is assigned to one or more users.",
                        statusCode: 400
                    );
                }
                              
                await _dbConnection.ExecuteAsync(
                    "DELETE FROM tblRolePermission WHERE RoleId = @RoleId",
                    new { RoleId = existingRole },
                    transaction
                );
                             
                var deleteRoleQuery = "DELETE FROM AspNetRoles WHERE Id = @RoleId";
                int rowsAffected = await _dbConnection.ExecuteAsync(deleteRoleQuery, new { RoleId = existingRole }, transaction);

                if (rowsAffected > 0)
                {
                    transaction.Commit();
                    return ExecutionResponse<string>.SuccessResponse(
                        message: $"Role '{deleteRoleRequestDto.Name}' deleted successfully.",
                        data: existingRole.ToString()
                    );
                }
                else
                {
                    transaction.Rollback();
                    return ExecutionResponse<string>.FailureResponse(
                        message: $"Failed to delete role '{deleteRoleRequestDto.Name}'.",
                        statusCode: 400
                    );
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ExecutionResponse<string>.FailureResponse(
                    message: $"Error deleting role '{deleteRoleRequestDto.Name}': {ex.Message}",
                    statusCode: 500
                );
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<GetRolePermissionDto>> GetRolePermissionMenusAsync(string? userName)
        {
            try
            {
                //string condition = "";
                //if (userName.StartsWith("LAN", StringComparison.OrdinalIgnoreCase))
                //{
                //    condition += $@"INNER JOIN AspNetUsers u ON u.RegistrationType LIKE 'LANDLORD'";
                //}
                //else if (userName.StartsWith("TEN", StringComparison.OrdinalIgnoreCase))
                //{
                //    condition += $@"INNER JOIN AspNetUsers u ON u.RegistrationType LIKE 'TENANT'";
                //}
                //else if (userName.StartsWith("AD", StringComparison.OrdinalIgnoreCase))
                //{
                //    condition += $@"INNER JOIN AspNetUsers u ON u.RegistrationType LIKE 'ADMIN'";
                //}           
                //else
                //{
                //    condition += $@"INNER JOIN AspNetUsers u ON u.RegistrationType LIKE 'AGENCY'";
                //}

                //string sqlQuery = $@"
                //  SELECT DISTINCT rp.PermissionID, p.AgencyID, p.MainMenuID, p.MainMenuName, p.SubMenuID, p.PermissionType,
                //  p.PermissionKey, p.PermissionName, p.IsActive, p.StateStatus, p.IsApproved ,IsAllowed=rp.IsAllowed,  rp.RolePermissionID, RoleId=CAST(roles.Id AS NVARCHAR(100)), UserId=CAST(aur.UserId AS NVARCHAR(100))
                //  FROM 
                //  tblRolePermission rp 
                //  INNER JOIN 
                //  tblPermission p ON p.PermissionID = rp.PermissionID   
                //  INNER JOIN AspNetRoles roles ON rp.RoleID = roles.Id 
                //  INNER JOIN AspNetUsers u ON roles.Id = u.RoleId AND rp.RoleID = u.RoleId  
                //  INNER JOIN AspNetUserRoles aur ON aur.RoleID = roles.Id  AND aur.UserId = u.RoleId  
                //  WHERE 1=1 AND rp.IsAllowed=1
                //  AND p.IsActive = 1 AND u.IsActive=1 AND u.UserName = @UserName
                //  ORDER BY p.SubMenuID, p.MainMenuID";
                string sqlQuery = $@"
                  SELECT DISTINCT rp.PermissionID, p.AgencyID, p.MainMenuID, p.MainMenuName, p.SubMenuID, p.PermissionType,
                  p.PermissionKey, p.PermissionName, p.IsActive, p.StateStatus, p.IsApproved ,IsAllowed=rp.IsAllowed,  rp.RolePermissionID, RoleId=CAST(roles.Id AS NVARCHAR(100)), UserId=CAST(u.Id AS NVARCHAR(100))
                  FROM 
                  tblRolePermission rp 
                  INNER JOIN 
                  tblPermission p ON p.PermissionID = rp.PermissionID   
                  INNER JOIN AspNetRoles roles ON rp.RoleID = roles.Id 
                  INNER JOIN AspNetUsers u ON roles.Id = u.RoleId AND rp.RoleID = u.RoleId   
                  WHERE 1=1 AND rp.IsAllowed=1
                  AND p.IsActive = 1 AND u.IsActive=1 AND u.UserName = @UserName
                  ORDER BY p.SubMenuID, p.MainMenuID";

                var parameters = new DynamicParameters();
                parameters.Add("UserName", userName);

                var data = await _dbConnection.QueryAsync<GetRolePermissionDto>(sqlQuery, parameters);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching permissions: {ex.Message}", ex);
            }
        }
        public async Task<(bool found, Guid roleId)> TryGetRoleIdAsync(string registrationType)
        {
            if (string.IsNullOrWhiteSpace(registrationType))
                throw new ArgumentException("Registration type cannot be null or empty.", nameof(registrationType));

            const string sql = """
            SELECT TOP 1 Id
            FROM   AspNetRoles
            WHERE  NormalizedName LIKE @NormalizedName
            """;

            var parameters = new DynamicParameters();
            parameters.Add("NormalizedName", $"%{registrationType}%");

            Guid? roleId = await _dbConnection.QueryFirstOrDefaultAsync<Guid?>(sql, parameters);

            return (roleId.HasValue && roleId != Guid.Empty, roleId ?? Guid.Empty);
        }

        
    }
}
