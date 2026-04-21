using Dapper;
using Domain.Models.Access.DomainModel;
using Domain.Models.Administrator.Login.Reset_Password;
using Domain.OtherModels.Response;
using Domain.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Application.IdentityObject
{
    public class CustomUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    {
        private readonly IDbConnection _dbConnection;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public CustomUserStore(IDbConnection dbConnection, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _dbConnection = dbConnection;
            _passwordHasher = passwordHasher;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string query = $@"
            INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, EmailConfirmed, Email, NormalizedEmail, Password,
             PasswordHash, SecurityStamp, IsActive, EmployeeId, EmployeeName, IsDefaultPassword, DefaultCode, DefaultPasswordHash, DefaultSecurityStamp, PhoneNumber, PhoneNumberConfirmed, ConcurrencyStamp, RegistrationType)
            VALUES (@Id, @UserName, @NormalizedUserName, @EmailConfirmed, @Email, @NormalizedEmail, @Password,
             @PasswordHash, @SecurityStamp, @IsActive, @EmployeeId, @EmployeeName, @IsDefaultPassword, @DefaultCode, @PasswordHash, @SecurityStamp, @PhoneNumber, @PhoneNumberConfirmed, @ConcurrencyStamp, @RegistrationType)";

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var rows = await _dbConnection.ExecuteAsync(query, user, transaction);

                transaction.Commit();
                return rows > 0 ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string query = "DELETE FROM AspNetUsers WHERE Id = @Id";
            var rows = await _dbConnection.ExecuteAsync(query, new { user.Id });

            return rows > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE Id = @Id AND IsActive=1";
            return await _dbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Id = Guid.Parse(userId) });
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";
            var data = await _dbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { NormalizedUserName = normalizedUserName });
            return data;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        //public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        //{
        //    const string query = "SELECT PasswordHash FROM AspNetUsers WHERE Id = @Id";
        //    return await _dbConnection.QuerySingleOrDefaultAsync<string>(query, new { user.Id });
        //}

        //public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        //{
        //    const string query = "SELECT CASE WHEN PasswordHash IS NOT NULL THEN 1 ELSE 0 END FROM AspNetUsers WHERE Id = @Id";
        //    var result = await _dbConnection.QuerySingleOrDefaultAsync<int>(query, new { user.Id });
        //    return result == 1;
        //}

        //public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        //{
        //    user.PasswordHash = passwordHash;
        //    return Task.CompletedTask;
        //}

        public async Task<ApplicationUser> FindByPassowrdAsync(string contactNumber, CancellationToken cancellationToken)
        {
            const string query = "SELECT PhoneNumber FROM AspNetUsers WHERE PhoneNumber = @PhoneNumber";
            return await _dbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { PhoneNumber = contactNumber });
        }
        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            const string query = $@"
                UPDATE AspNetUsers
                SET PhoneNumber = @PhoneNumber,
                    UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    Email = @Email,
                    NormalizedEmail = @NormalizedEmail,
                    Password = @Password,
                    PasswordHash = @PasswordHash,
                    SecurityStamp = @SecurityStamp,                
                    RefreshToken = @RefreshToken,
                    RefreshTokenExpiryTime = @RefreshTokenExpiryTime,
                    IsDefaultPassword = @IsDefaultPassword,
                    PasswordChangedCount = @PasswordChangedCount
                    WHERE Id = @Id";
            try
            {
                var rows = await _dbConnection.ExecuteAsync(query, new
                {
                    user.PhoneNumber,
                    user.UserName,
                    user.NormalizedUserName,
                    user.Email,
                    user.NormalizedEmail, user.Password,
                    user.PasswordHash,
                    user.SecurityStamp,                
                    user.RefreshToken,
                    user.RefreshTokenExpiryTime,
                    user.Id,
                    user.IsDefaultPassword,
                    user.PasswordChangedCount
                });

                return rows > 0 ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Update failed: {ex.Message}" });
            }
        }

        public async Task<(string RefreshToken, DateTime ExpiryTime)?> GetUserRefreshTokenAsync(string normalizedUserName)
        {
            const string query = @"SELECT RefreshToken, RefreshTokenExpiryTime
                           FROM AspNetUsers
                           WHERE NormalizedUserName = @NormalizedUserName";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<dynamic>(query, new { NormalizedUserName = normalizedUserName });
            if (result == null)
                return null;

            string refreshToken = result.RefreshToken;
            DateTime expiry = result.RefreshTokenExpiryTime;
            return (refreshToken, expiry);
        }

        public async Task UpdateUserRefreshTokenAsync(string userId, string newRefreshToken, DateTime expiryTime)
        {
            const string query = @"UPDATE AspNetUsers
                           SET RefreshToken = @RefreshToken,
                               RefreshTokenExpiryTime = @ExpiryTime
                           WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(query, new { RefreshToken = newRefreshToken, ExpiryTime = expiryTime, Id = userId });
        }

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE Email = @Email";
            var data = await _dbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Email = email });
            return data;
        }

        // Password store
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
            => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        // Email store
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }      

        public void Dispose()
        {
            // Nothing to dispose
        }


    }

}
