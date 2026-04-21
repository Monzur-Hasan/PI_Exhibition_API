using Domain.Models.Administrator.Login.Request;
using Domain.OtherModels.EmailService;
using Domain.OtherModels.Response;
using Domain.ViewModels.Access;
using Domain.ViewModels.OTP;

namespace Infrastructure.Features.Service.Administrator
{
    public interface ILoginManager
    {
        Task<AppUserLoggedInfo> GetAppUserLogInfo2Async(string email);
        Task<AppUserLoggedInfo> GetAppUserLoggedInfoAsync(string email);
        Task<bool> IsEmailExistAsync(string email);

        Task<AppUserLoggedInfo> GetAppUserEmployeeInfoAsync(long? employeeId, long? companyId, long? organizationId, string? database);
        Task<IEnumerable<LoginRequestDto>> GetLoginInfosAsync(long? companyId);
        Task<EmailSettingObject> EmailSettings(string EmaliFor);
        Task<ExecutionStatus> UserForgetPasswordOTPResquestAsync(OTPRequestsViewModel model);
        Task<ExecutionStatus> UserForgetPasswordOTPVerificationAsync(OTPVerificationViewModel model);
    }
}
