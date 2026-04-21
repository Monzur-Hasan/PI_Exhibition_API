using Dapper;
using Domain.Models.Access.DomainModel;
using Domain.Models.Administrator.Login.Request;
using Domain.OtherModels.EmailService;
using Domain.OtherModels.Response;
using Domain.Shared.Configurations;
using Domain.Shared.Helpers;
using Domain.ViewModels.Access;
using Domain.ViewModels.OTP;
using Infrastructure.Features.Service.Administrator;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Net;
using System.Net.Mail;


namespace Application.Features.Service.Administrator
{
    public class LoginManager : ILoginManager
    {     
        private UserManager<ApplicationUser> _userManager;
        private IDbConnection _dbConnection;
  
        public LoginManager(UserManager<ApplicationUser> userManager, IDbConnection dbConnection,
            SignInManager<ApplicationUser> signInManager)
        {  
          
            _userManager = userManager;
        
        }

        public async Task<AppUserLoggedInfo> GetAppUserLoggedInfoAsync(string username)
        {
            AppUserLoggedInfo data = null;
            var sp_name = "sp_AppUserLoggedInfo";  // stored procedure name
            var parameters = new DynamicParameters();
            parameters.Add("Username", username); // Add parameters to the dynamic object

            try
            {
                if (!string.IsNullOrWhiteSpace(sp_name))
                {
                    data = await _dbConnection.QueryFirstAsync<AppUserLoggedInfo>(sp_name, parameters);
                   
                }
            }
            catch (Exception ex)
            {
                
            }

            return data;
        }
        public async Task<bool> IsEmailExistAsync(string email)
        {
            bool IsExist = false;
            try
            {
                IsExist = await _userManager.FindByEmailAsync(email) != null;
            }
            catch (Exception ex)
            {
                
            }

            return IsExist;
        }
        public async Task<ExecutionStatus> UserForgetPasswordOTPResquestAsync(OTPRequestsViewModel model)
        {
            ExecutionStatus executionStatus = new ExecutionStatus();
            try
            {
                var appUser = await _userManager.FindByEmailAsync(model.Email);
                if (appUser != null)
                {
                    var sp_name = "sp_OTPRequests";

                    var otp = UtilityService.GenerateRandomDigits(5, UtilityService.numericCharacters);
                    var parameters = new DynamicParameters();
                    parameters.Add("Email", model.Email);
                    parameters.Add("PublicIP", model.PublicIP);
                    parameters.Add("PrivateIP", model.PrivateIP);
                    parameters.Add("DeviceType", model.DeviceType);
                    parameters.Add("OS", model.OS);
                    parameters.Add("OSVersion", model.OSVersion);
                    parameters.Add("Browser", model.Browser);
                    parameters.Add("BrowserVersion", model.BrowserVersion);
                    parameters.Add("OTP", otp);
                    parameters.Add("ExecutionFlag", "Request");

                    executionStatus = await _dbConnection.QueryFirstAsync<ExecutionStatus>(sp_name, parameters);
                    if (executionStatus != null && executionStatus.Status)
                    {
                        var emailSetting = UtilityService.JsonToObject<IEnumerable<EmailSettingObject>>(executionStatus.Json).FirstOrDefault();
                        if (emailSetting != null)
                        {
                            await OTPEmailService(model.Email, otp, emailSetting, EmailTemplateFlag.ForgetPassword);
                            executionStatus.token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                        }
                        else
                        {
                            executionStatus = ResponseMessage.Invalid("Can not find user with this email");

                        }
                    }
                    else
                    {
                        executionStatus = ResponseMessage.Invalid("Can not find user with this email");
                    }
                }
            }
            catch (Exception ex)
            {
               executionStatus = ResponseMessage.Invalid(ResponseMessage.SomthingWentWrong);
            }
            return executionStatus;
        }
        private async Task OTPEmailService(string toEmail, string password, EmailSettingObject emailSetting, string flag = "")
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(emailSetting.EmailAddress, emailSetting.DisplayName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = emailSetting.Subject;
                message.IsBodyHtml = emailSetting.IsBodyHtml;
                message.Body = UtilityService.GetEmailTemplate(flag, password);

                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = emailSetting.EnableSsl;
                smtp.UseDefaultCredentials = emailSetting.UseDefaultCredentials;
                smtp.Port = Convert.ToInt32(emailSetting.Port);
                smtp.Host = emailSetting.Host;
                smtp.Credentials = new NetworkCredential(emailSetting.EmailAddress, emailSetting.EmailPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                
            }
        }
        public async Task<ExecutionStatus> UserForgetPasswordOTPVerificationAsync(OTPVerificationViewModel model)
        {
            ExecutionStatus executionStatus = null;
            try
            {
                var sp_name = "sp_OTPRequests";
                var parameters = new DynamicParameters();
                parameters.Add("Email", model.Email);
                parameters.Add("OTP", model.OTP);
                parameters.Add("ExecutionFlag", "Verification");

                executionStatus = await _dbConnection.QueryFirstAsync<ExecutionStatus>(sp_name, parameters);
                if (executionStatus.Status)
                {
                    var appUser = await _userManager.FindByEmailAsync(model.Email);
                    var newPassword = UtilityService.RandomPassword();
                    var result = await _userManager.ResetPasswordAsync(appUser, model.Token, newPassword);
                    if (result.Succeeded)
                    {
                        executionStatus.Msg = "OTP varified Successfully.<br> Defualt password has been sent to your email. <br> Please login with this password";

                        var emailSetting = UtilityService.JsonToObject<IEnumerable<EmailSettingObject>>(executionStatus.Json).FirstOrDefault();

                        if (emailSetting != null)
                        {
                            await OTPEmailService(model.Email, newPassword, emailSetting, EmailTemplateFlag.DefaultPassword);
                        }
                        else
                        {
                            executionStatus.Status = false;
                            executionStatus.ErrorMsg = "Can not find user with this email";
                        }
                    }
                    else
                    {
                        executionStatus.Status = false;
                        executionStatus.Msg = "Reset is failed due to invalid token";
                    }
                }

            }
            catch (Exception ex)
            {
                executionStatus = UtilityService.Invalid(ResponseMessage.SomthingWentWrong);
            }
            return executionStatus;
        }
        public async Task<EmailSettingObject> EmailSettings(string EmaliFor)
        {
            EmailSettingObject emailSetting = null;
            try
            {
                var sp_name = "sp_EmailSettingInformation";
                var parameters = new DynamicParameters();
                parameters.Add("EmailFor", EmaliFor);
                emailSetting = await _dbConnection.QueryFirstAsync<EmailSettingObject>(sp_name, parameters);
            }
            catch (Exception ex)
            {
                
            }
            return emailSetting;
        }
        public async Task<AppUserLoggedInfo> GetAppUserEmployeeInfoAsync(long? employeeId, long? companyId, long? organizationId, string? database)
        {
            AppUserLoggedInfo info = new AppUserLoggedInfo();
            try
            {
                var query = $@"Select emp.*,EmployeeName=emp.FullName,grade.GradeName,desig.DesignationName,dept.DepartmentName,
                sec.SectionName,subsec.SubSectionName,JobStatusName=(CASE WHEN ISNULL(emp.IsActive,0)=0 THEN 'Inactive' 
                WHEN ISNULL(emp.IsActive,0)=1 THEN 'Active' ELSE 'Inactive' END),
                (ws.Title+'#'+ws.[Name]) 'WorkShiftName',emp.WorkShiftId,
                PhotoPath= ISNULL(SUBSTRING((ED.PhotoPath+'/'+ED.Photo),CHARINDEX('/',(ED.PhotoPath+'/'+ED.Photo))+1,200),'default'),emp.TerminationDate
                From HR_EmployeeInformation emp
                LEFT Join HR_Designations Desig on emp.DesignationId = desig.DesignationId
                LEFT Join HR_Grades grade on Desig.GradeId = grade.GradeId
                Left Join HR_Departments dept on emp.DepartmentId = dept.DepartmentId
                Left Join HR_Sections sec on emp.SectionId = sec.SectionId
                Left Join HR_SubSections subsec on emp.SubSectionId = subsec.SubSectionId
                Left Join HR_WorkShifts ws on emp.WorkShiftId = ws.WorkShiftId
                LEFT Join HR_EmployeeDetail ED ON emp.EmployeeId = ed.EmployeeId
                Where 1=1
                AND emp.EmployeeId=@EmployeeId 
                AND emp.CompanyId=@CompanyId 
                AND emp.OrganizationId=@OrganizationId";
                var parameters = new { EmployeeId = employeeId, CompanyId = companyId, OrganizationId = organizationId };
                info = await _dbConnection.QueryFirstAsync<AppUserLoggedInfo>(query, parameters);
            }
            catch (Exception ex)
            {
               
            }
            return info;
        }
        public async Task<AppUserLoggedInfo> GetAppUserLogInfo2Async(string email)
        {
            var userInfo = new AppUserLoggedInfo();
            try
            {
                var query = $@"Select
                Cast(u.Id as Nvarchar(50)) 'UserId',u.UserName 'Username',Cast(r.Id as Nvarchar(50)) 'RoleId',
                SUBSTRING(r.[Name],1,(CASE WHEN CHARINDEX('#Org',r.[Name]) > 0 
                THEN CHARINDEX('#Org',r.[Name])-1 ELSE LEN(r.[NAME]) END)) as 'RoleName',
                (Case 
                When PasswordExpiredDate IS NULL then 0
                When PasswordExpiredDate < Cast(GETDATE() as date) then 0
                When PasswordExpiredDate > Cast(GETDATE() as date) then DATEDIFF(DAY,GETDATE(),PasswordExpiredDate)
                When PasswordExpiredDate = Cast(GETDATE() as date) then 0
                Else 0 End) 'RemainExpireDays',u.IsActive,
               -- [SiteThumbnailPath]=SUBSTRING(com.CompanyLogoPath,CHARINDEX('/',com.CompanyLogoPath)+1,500), 
                --SiteShortName=Com.ShortName,Com.CompanyName,Org.OrganizationName,
				u.EmployeeId,u.CompanyId,u.OrganizationId,u.IsDefaultPassword
                From AspNetUsers u
                Inner Join AspNetRoles r On u.RoleId = r.Id
                --Inner Join tblOrganizations Org On Org.OrganizationId = u.OrganizationId
                --Inner Join tblCompanies Com On Com.CompanyId = U.CompanyId
                Where 1=1 AND u.Email = @Email";
                userInfo = await _dbConnection.QueryFirstAsync<AppUserLoggedInfo>(query, new { Email = email });
                
            }
            catch (Exception ex)
            {
                
            }
            return userInfo;
        }
        public async Task<IEnumerable<LoginRequestDto>> GetLoginInfosAsync(long? companyId)
        {
            IEnumerable<LoginRequestDto> list = new List<LoginRequestDto>();
            try
            {
                var query = $@"SELECT UserName,[Password]='M0nzurH@s@n' FROM AspNetUsers
                Where CompanyId=@CompanyId";
                list = (IEnumerable<LoginRequestDto>) await _dbConnection.QueryFirstAsync<LoginRequestDto>(query, new { CompanyId = companyId });
            }
            catch (Exception ex)
            {
                
            }
            return list;
        }


    }
}
