using Application.DapperObject;
using Dapper;
using Domain.Models.Access.DomainModel;
using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Login.Request;
using Domain.Models.Administrator.Login.Result;
using Domain.OtherModels.Response;
using Domain.Shared.Settings;
using Domain.Shared.Settings.Jwt_Settings;
using Infrastructure.Features.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;

namespace Application.Features.Repository.Implementation
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ProjectRepository> _logger;
        private readonly JwtSettings _jwt;

        public ProjectRepository(DapperContext context, ILogger<ProjectRepository> logger, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _logger = logger;
            _jwt = jwtOptions.Value;
        }


        public async Task<long> RegisterAsync(RegisterRequestDto request, List<string> imagePaths)
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Hash password
                var passwordHash = string.Empty;
                if (!string.IsNullOrEmpty(request.Password))
                {
                    var hasher = new PasswordHasher<RegisterRequestDto>();
                    passwordHash = hasher.HashPassword(request, request.Password);
                }

                // Insert user
                var userQuery = @"
                INSERT INTO Users (Name, PhoneNumber, Address, Email, PasswordHash)
                VALUES (@Name, @PhoneNumber, @Address, @Email, @PasswordHash);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

                var userId = await connection.ExecuteScalarAsync<long>(
                    userQuery,
                    new
                    {
                        request.Name,
                        request.PhoneNumber,
                        request.Address,
                        request.Email,
                        PasswordHash = passwordHash ?? null
                    },
                    transaction);

                // Insert project
                var projectQuery = @"
                INSERT INTO Projects 
                (UserId, ProjectLocation, ProjectMeasurement, Problem, Comments)
                VALUES 
                (@UserId, @ProjectLocation, @ProjectMeasurement, @Problem, @Comments);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

                var projectId = await connection.ExecuteScalarAsync<long>(
                    projectQuery,
                    new
                    {
                        UserId = userId,
                        request.ProjectLocation,
                        request.ProjectMeasurement,
                        request.Problem,
                        request.Comments
                    },
                    transaction);

                // Insert images
                if (imagePaths != null && imagePaths.Count > 0)
                {
                    var imageQuery = @"INSERT INTO ProjectImages (ProjectId, ImageUrl)
                               VALUES (@ProjectId, @ImageUrl)";

                    foreach (var path in imagePaths)
                    {
                        await connection.ExecuteAsync(
                            imageQuery,
                            new { ProjectId = projectId, ImageUrl = path },
                            transaction);
                    }
                }

                transaction.Commit();
                return projectId;
            }
            catch (Exception)
            {
                transaction.Rollback();
                foreach (var path in imagePaths)
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }

                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                 var query = @"
                UPDATE Users SET
                Name = @Name,
                PhoneNumber = @PhoneNumber,
                Email = @Email,
                Address = @Address
                WHERE Id = @Id";

                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new
                    {
                        user.Id,
                        user.Name,
                        user.PhoneNumber,
                        user.Email,
                        user.Address
                    },
                    transaction
                );

                transaction.Commit();

                return affectedRows > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> CheckPhoneNumberExistsAsync(string phoneNumber)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE PhoneNumber = @PhoneNumber";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { PhoneNumber = phoneNumber });
            return count > 0;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<UserDto> GetUserByPhoneNumberAsync(string phoneNumber)
       {
            var query = @"
        SELECT 
            u.Id,
            u.Name,
            u.PhoneNumber,
            u.Email,
            u.Address,
            p.ProjectLocation, p.ProjectMeasurement,
            p.Problem,
            p.Comments,
            pi.ImageUrl
        FROM Users u
        LEFT JOIN Projects p ON u.Id = p.UserId
        LEFT JOIN ProjectImages pi ON p.Id = pi.ProjectId
        WHERE u.PhoneNumber = @PhoneNumber";

            using var connection = _context.CreateConnection();

            UserDto user = null;

            var result = await connection.QueryAsync<UserDto, string, UserDto>(
                query,
                (u, imageUrl) =>
                {
                    // First row → create user
                    if (user == null)
                    {
                        user = u;
                        user.ImageUrls = new List<string>();
                    }

                    // Add images
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        user.ImageUrls.Add(imageUrl);
                    }

                    return user;
                },
                new { PhoneNumber = phoneNumber },
                splitOn: "ImageUrl"
            );

            return user;
        }

        public async Task SaveRefreshTokenAsync(long userId, string refreshToken, string deviceInfo)
        {
            var query = @"
            INSERT INTO UserRefreshTokens (UserId, Token, ExpiryDate, DeviceInfo)
            VALUES (@UserId, @Token, @ExpiryDate, @DeviceInfo)";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(query, new
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(AppSettings.RefreshTokenExpirationDays), // FIX
                DeviceInfo = deviceInfo
            });
        }
        public async Task<UserRefreshToken?> GetRefreshTokenAsync(string token)
        {
            var query = @"
                SELECT Id, UserId, Token, ExpiryDate, IsRevoked
                FROM UserRefreshTokens
                WHERE Token = @Token";

            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserRefreshToken>(
                query,
                new { Token = token }
            );
        }

        public async Task RevokeRefreshTokenAsync(long id)
        {
            var query = @"
            UPDATE UserRefreshTokens
            SET IsRevoked = 1,
                ExpiryDate = GETUTCDATE()
            WHERE Id = @Id";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task RevokeAllTokensAsync(long userId)
        {
            var query = @"
            UPDATE UserRefreshTokens
            SET IsRevoked = 1
            WHERE Id = @Id";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(query, new { Id = userId });
        }

        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            var query = @"
                SELECT 
                    p.Id, p.UserId, p.ProjectLocation, p.ProjectMeasurement, p.Problem, p.Comments, p.CreatedAt,
                    u.Name, u.PhoneNumber, u.Address, u.Email,
                    pi.ImageUrl
                FROM Projects p
                INNER JOIN Users u ON p.UserId = u.Id
                LEFT JOIN ProjectImages pi ON p.Id = pi.ProjectId";

            using var connection = _context.CreateConnection();

            var projectDict = new Dictionary<long, ProjectDto>();

            var result = await connection.QueryAsync<ProjectDto, string, ProjectDto>(
                query,
                (project, imageUrl) =>
                {
                    if (!projectDict.TryGetValue(project.Id, out var currentProject))
                    {
                        currentProject = project;
                        currentProject.ImageUrls = new List<string>();
                        projectDict.Add(currentProject.Id, currentProject);
                    }

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        currentProject.ImageUrls.Add(imageUrl);
                    }

                    return currentProject;
                },
                splitOn: "ImageUrl"
            );

            return projectDict.Values.ToList();
        }

        public async Task<ProjectDto> GetProjectByIdAsync(long id)
        {
            var query = @"
            SELECT 
                p.Id, p.UserId, p.ProjectLocation, p.ProjectMeasurement, p.Problem, p.Comments, p.CreatedAt,
                u.Name, u.PhoneNumber, u.Address, u.Email,
                pi.Id as ImageId, pi.ImageUrl
            FROM Projects p
            INNER JOIN Users u ON p.UserId = u.Id
            LEFT JOIN ProjectImages pi ON p.Id = pi.ProjectId
            WHERE p.Id = @Id
    ";

            using var connection = _context.CreateConnection();

            var dict = new Dictionary<long, ProjectDto>();

            var result = await connection.QueryAsync<ProjectDto, dynamic, ProjectDto>(
                query,
                (project, image) =>
                {
                    if (!dict.TryGetValue(project.Id, out var current))
                    {
                        current = project;
                        current.ImageUrls = new List<string>();
                        dict.Add(current.Id, current);
                    }

                    if (image?.ImageUrl != null)
                        current.ImageUrls.Add(image.ImageUrl);

                    return current;
                },
                new { Id = id },
                splitOn: "ImageId"
            );

            return dict.Values.FirstOrDefault();
        }

        public async Task<UserDto> GetuserByIdAsync(long id)
        {
            var query = @"SELECT Id, [Name], PhoneNumber, Email, [Address] FROM Users
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<UserDto>(query, new { Id = id });
            return result;
        }

        public async Task UpdateProjectAsync(long id, UpdateProjectRequestDto request)
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var newImagePaths = new List<string>();

            try
            {
                var updateQuery = @"
        UPDATE Projects SET
            ProjectLocation = @ProjectLocation,
            ProjectMeasurement = @ProjectMeasurement,
            Problem = @Problem,
            Comments = @Comments
        WHERE Id = @Id";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = id,
                    request.ProjectLocation,
                    request.ProjectMeasurement,
                    request.Problem,
                    request.Comments
                }, transaction);

                if (request.DeleteImageIds != null && request.DeleteImageIds.Any())
                {                    
                    var selectQuery = "SELECT ImageUrl FROM ProjectImages WHERE Id IN @Ids";

                    var oldImages = (await connection.QueryAsync<string>(
                        selectQuery,
                        new { Ids = request.DeleteImageIds },
                        transaction)).ToList();

  
                    var deleteQuery = "DELETE FROM ProjectImages WHERE Id IN @Ids";

                    await connection.ExecuteAsync(deleteQuery,
                        new { Ids = request.DeleteImageIds },
                        transaction);

                    foreach (var path in oldImages)
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }

                if (request.NewImages != null && request.NewImages.Count > 0)
                {
                    var insertQuery = @"INSERT INTO ProjectImages (ProjectId, ImageUrl)
                                VALUES (@ProjectId, @ImageUrl)";

                    foreach (var file in request.NewImages)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine("Uploads", fileName);

                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);

                        newImagePaths.Add(path);

                        await connection.ExecuteAsync(insertQuery,
                            new
                            {
                                ProjectId = id,
                                ImageUrl = path
                            },
                            transaction);
                    }
                }
               
                transaction.Commit();
            }
            catch (Exception)
            {              
                transaction.Rollback();

                foreach (var path in newImagePaths)
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }

                throw;
            }
        }

        public async Task DeleteProjectAsync(long id)
        {
            using var connection = _context.CreateConnection();

            // delete images first
            await connection.ExecuteAsync(
                "DELETE FROM ProjectImages WHERE ProjectId = @Id",
                new { Id = id });

            // delete project
            await connection.ExecuteAsync(
                "DELETE FROM Projects WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<UpsertUserProjectResult> UpsertUserProjectAsync(
            RegisterRequestDto request,
            List<string> imagePaths)
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var newImagePaths = new List<string>();

            try
            {
                // 🔴 CHECK EMAIL EXIST (IMPORTANT RULE)
                var emailExists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Users WHERE Email = @Email AND PhoneNumber <> @PhoneNumber",
                    new { request.Email, request.PhoneNumber },
                    transaction
                );

                if (emailExists > 0)
                {
                    transaction.Rollback();

                    return new UpsertUserProjectResult
                    {
                        Success = false,
                        Message = "Email already exists",
                        ProjectId = 0
                    };
                }

                // 🔍 Check existing user
                var existingUser = await connection.QueryFirstOrDefaultAsync<UserDto>(
                    "SELECT * FROM Users WHERE PhoneNumber = @PhoneNumber",
                    new { request.PhoneNumber },
                    transaction
                );

                long userId;
                long projectId;

                if (existingUser == null)
                {
                    // ======================
                    // INSERT USER
                    // ======================
                    var passwordHash = string.Empty;

                    if (!string.IsNullOrEmpty(request.Password))
                    {
                        var hasher = new PasswordHasher<RegisterRequestDto>();
                        passwordHash = hasher.HashPassword(request, request.Password);
                    }

                    userId = await connection.ExecuteScalarAsync<long>(
                        @"INSERT INTO Users (Name, PhoneNumber, Address, Email, PasswordHash)
                  VALUES (@Name, @PhoneNumber, @Address, @Email, @PasswordHash);
                  SELECT CAST(SCOPE_IDENTITY() as bigint);",
                        new
                        {
                            request.Name,
                            request.PhoneNumber,
                            request.Address,
                            request.Email,
                            PasswordHash = passwordHash
                        },
                        transaction
                    );

                    // ======================
                    // INSERT PROJECT
                    // ======================
                    projectId = await connection.ExecuteScalarAsync<long>(
                        @"INSERT INTO Projects 
                  (UserId, ProjectLocation, ProjectMeasurement, Problem, Comments)
                  VALUES 
                  (@UserId, @ProjectLocation, @ProjectMeasurement, @Problem, @Comments);
                  SELECT CAST(SCOPE_IDENTITY() as bigint);",
                        new
                        {
                            UserId = userId,
                            request.ProjectLocation,
                            request.ProjectMeasurement,
                            request.Problem,
                            request.Comments
                        },
                        transaction
                    );
                }
                else
                {
                    // ======================
                    // UPDATE USER
                    // ======================
                    userId = existingUser.Id;

                    await connection.ExecuteAsync(
                        @"UPDATE Users SET 
                    Name = @Name,
                    Email = @Email,
                    Address = @Address
                  WHERE Id = @Id",
                        new
                        {
                            Id = userId,
                            request.Name,
                            request.Email,
                            request.Address
                        },
                        transaction
                    );

                    // ======================
                    // GET PROJECT
                    // ======================
                    projectId = await connection.ExecuteScalarAsync<long>(
                        "SELECT Id FROM Projects WHERE UserId = @UserId",
                        new { UserId = userId },
                        transaction
                    );

                    // ======================
                    // UPDATE PROJECT
                    // ======================
                    await connection.ExecuteAsync(
                        @"UPDATE Projects SET
                    ProjectLocation = @ProjectLocation,
                    ProjectMeasurement = @ProjectMeasurement,
                    Problem = @Problem,
                    Comments = @Comments
                  WHERE Id = @Id",
                        new
                        {
                            Id = projectId,
                            request.ProjectLocation,
                            request.ProjectMeasurement,
                            request.Problem,
                            request.Comments
                        },
                        transaction
                    );

                    // ======================
                    // DELETE OLD IMAGES
                    // ======================
                    var oldImages = (await connection.QueryAsync<string>(
                        "SELECT ImageUrl FROM ProjectImages WHERE ProjectId = @ProjectId",
                        new { ProjectId = projectId },
                        transaction
                    )).ToList();

                    await connection.ExecuteAsync(
                        "DELETE FROM ProjectImages WHERE ProjectId = @ProjectId",
                        new { ProjectId = projectId },
                        transaction
                    );

                    foreach (var path in oldImages)
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }

                // ======================
                // INSERT NEW IMAGES
                // ======================
                if (imagePaths != null && imagePaths.Count > 0)
                {
                    foreach (var path in imagePaths)
                    {
                        await connection.ExecuteAsync(
                            @"INSERT INTO ProjectImages (ProjectId, ImageUrl)
                      VALUES (@ProjectId, @ImageUrl)",
                            new { ProjectId = projectId, ImageUrl = path },
                            transaction
                        );
                    }
                }

                transaction.Commit();

                return new UpsertUserProjectResult
                {
                    Success = true,
                    Message = "Saved successfully",
                    ProjectId = projectId
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                foreach (var path in newImagePaths)
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }

                return new UpsertUserProjectResult
                {
                    Success = false,
                    Message = ex.Message,
                    ProjectId = 0
                };
            }
        }


    }
}
