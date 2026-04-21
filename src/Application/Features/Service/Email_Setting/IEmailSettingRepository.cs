using Domain.OtherModels.EmailService;

namespace Application.Features.Service.Email_Setting
{
    public interface IEmailSettingRepository
    {
        Task<EmailSettingObject> GetEmailSettingAsync(string emailFor);
    }
}
