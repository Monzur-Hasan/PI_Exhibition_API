using Dapper;
using Domain.Models.Access.DomainModel;
using Domain.Models.Administrator.Login.Reset_Password;
using Domain.OtherModels.EmailService;
using Domain.OtherModels.Response;
using Domain.Shared.Helpers;
using Domain.ViewModels.OTP;
using Microsoft.AspNetCore.Identity;
using System.Data;
namespace Application.IdentityObject
{
    public class ForgotPasswordEmailValidator
    {
        private readonly IDbConnection _dbConnection;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public ForgotPasswordEmailValidator(IDbConnection dbConnection, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _dbConnection = dbConnection;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> FindByEmailAddressAsync(string email)
        {
            const string query = $@"SELECT COUNT(1) FROM AspNetUsers WHERE Email = @Email";
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<OTPVerification> GetOTPInfoAsync(string otp, string email)
        {
            const string query = @"
            SELECT TOP 1 OTPLifeTime, Email,VerifiedTime
            FROM tblOTPRequests
            WHERE Email = @Email AND OTP = @OTP";
            return await _dbConnection.QueryFirstOrDefaultAsync<OTPVerification>(query, new { Email = email, OTP = otp });
        }

        public async Task<bool> UpdateSendOTPTimeAsync(string email, string otp)
        {
            await Task.Delay(5);

            _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string query = $@"
                UPDATE tblOTPRequests
                SET VerifiedTime = GETDATE(), IsVerified = 1
                WHERE Email = @Email AND OTP = @OTP";

                int affected = await _dbConnection.ExecuteAsync(query, new
                {
                    Email = email,
                    OTP = otp
                }, transaction);

                if (affected > 0)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error occurred while saving info.", ex);
            }
            finally
            {
                _dbConnection.Close();
            }
        }


        public async Task<EmailSettingObject> GetEmailSettingAsync(string emailFor)
        {
            const string query = "SELECT * FROM tblEmailSetting WHERE EmailFor = @EmailFor";
            return await _dbConnection.QueryFirstOrDefaultAsync<EmailSettingObject>(query, new { EmailFor = emailFor });
        }
        public async Task<ExecutionStatus> SaveOTPRequestsAsync(OTPRequestsViewModel oTPRequestsViewModel)
        {
            Thread.Sleep(5);
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                int otpExpiredTime = 3;

                string query = $@"INSERT INTO tblOTPRequests ([RequestUniqId], [Email], [PublicIP], [PrivateIP], 
                [DeviceType], [OS], [OSVersion], [Browser], [BrowserVersion], [IsVerified], [OTP], [OTPLifeTime], [VerifiedTime], [CreatedDate]) 
                VALUES(NEWID(), @Email, @PublicIP, @PrivateIP, @DeviceType, @OS, @OSVersion, @Browser, @BrowserVersion, 0, @OTP, DATEADD(MINUTE, {otpExpiredTime}, GETDATE()), NULL, GETDATE())";

                var rowCountUser = await _dbConnection.ExecuteAsync(query, oTPRequestsViewModel, transaction);


                if (rowCountUser > 0)
                {
                    transaction.Commit();
                    return new ExecutionStatus
                    {
                        Status = true,
                        Msg = "Success",
                        Code = "200"
                    };
                }
                return new ExecutionStatus
                {
                    Status = false,
                    Msg = "Failed",
                    Code = "400"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error occurred while saving info.", ex);
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<OTPVerification> GetOTPExpirationTimeAsync(string email)
        {
            var query = $@"SELECT TOP 1 [OTPLifeTime], [CreatedDate] FROM tblOTPRequests WHERE 1=1 AND Email = @Email
                ORDER BY OTPLifeTime DESC";
            var data = await _dbConnection.QueryFirstOrDefaultAsync<OTPVerification>(query, new { Email = email });
            return data;
        }

        //public async Task<bool> FindByPasswordHashAsync(Guid id, string currentPasswordHash)
        //{
        //    string query = "SELECT DefaultPasswordHash, PasswordHash FROM AspNetUsers WHERE Id = @Id AND (DefaultPasswordHash = @currentPasswordHash OR PasswordHash = @currentPasswordHash)";
        //    var data = await _dbConnection.QueryFirstOrDefaultAsync<bool>(query, new { id , currentPasswordHash});
        //    if (data)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public async Task<ApplicationUser> GetPasswordHashAsync(Guid id)
        {
            const string query = "SELECT PasswordHash, DefaultPasswordHash FROM AspNetUsers WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Id= id });
        }

        public async Task<string> GetDefaultPasswordAsync(Guid id)
        {
            const string query = "SELECT [DefaultPassword]=DefaultCode FROM AspNetUsers WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<string>(query, new { Id = id });
        }

        public async Task<ResetPasswordDto> FindByEmailForResetPasswordAsync(string email)
        {
            string query = $@"SELECT PasswordChangedCount, Email, UserName FROM AspNetUsers WHERE Email = @Email";
            var data = await _dbConnection.QueryFirstAsync<ResetPasswordDto>(query, new { Email = email });
            return data;

        }
        public async Task<string> GetPhoneNumberAsync(string phoneNumber)
        {
            string query = "SELECT PhoneNumber FROM AspNetUsers WHERE PhoneNumber = @PhoneNumber";
            return await _dbConnection.QuerySingleOrDefaultAsync<string>(query, new { PhoneNumber = phoneNumber });
        }

        public async Task<ExecutionStatus> UpdatePasswordAsync(ResetPasswordDto resetPasswordDto)
        {          
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                Guid concurrencyStamp = Guid.NewGuid();
                {
                    string userQuery = $@"
                           UPDATE AspNetUsers SET [Password]= @Password, PasswordHash = @PasswordHash, DefaultCode = @Password, DefaultPasswordHash = @PasswordHash, UpdatedBy = @Email, UpdatedDate = GETDATE(),
                            SecurityStamp = @SecurityStamp, DefaultSecurityStamp = @DefaultSecurityStamp, ConcurrencyStamp = @ConcurrencyStamp, PasswordChangedCount = @PasswordChangedCount, IsDefaultPassword=1
                            WHERE 1=1 AND Email = @Email";

                    var app_user = new ApplicationUser
                    {
                        Email = resetPasswordDto.Email,
                        UserName = resetPasswordDto.UserName ?? resetPasswordDto.Email
                    };

                    string passwordHash = _passwordHasher.HashPassword(app_user, resetPasswordDto.Password);

                    var user = new
                    {
                        Email = resetPasswordDto.Email,
                        PasswordChangedCount = resetPasswordDto.PasswordChangedCount + 1,
                        DefaultCode = resetPasswordDto.Password,
                        //DefaultPasswordHash = UtilityService.HashPassword(resetPasswordDto.Password),
                        DefaultPasswordHash = passwordHash,
                        DefaultSecurityStamp = Guid.NewGuid().ToString(),
                        Password = resetPasswordDto.Password,
                        //PasswordHash = UtilityService.HashPassword(resetPasswordDto.Password),
                        PasswordHash = passwordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = concurrencyStamp
                    };
                    var rowCount = await _dbConnection.ExecuteAsync(userQuery, user, transaction);
                    if (rowCount > 0)
                    {
                        transaction.Commit();
                        return new ExecutionStatus
                        {
                            Status = true,
                            Msg = "Success",
                            Code = "200",
                        };
                    }
                }

                return new ExecutionStatus
                {
                    Status = false,
                    Msg = "Failed",
                    Code = "400"
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error occurred while edit password.", ex);
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            const string query = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(
                query,
                new { Id = userId }
            );
        }

        public async Task<IdentityResult> UpdateUserInfoAsync(ApplicationUser user)
        {
            const string query = @"
            UPDATE AspNetUsers SET
                EmployeeName = @EmployeeName,              
                Email = @Email,
                NormalizedEmail = @NormalizedEmail,
                Address = @Address,        
                UpdatedBy = @UpdatedBy,
                UpdatedDate = @UpdatedDate
            WHERE Id = @Id";

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var rows = await _dbConnection.ExecuteAsync(query, user);

            return rows > 0
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError
                {
                    Description = "User update failed"
                });
        }
    }
}


