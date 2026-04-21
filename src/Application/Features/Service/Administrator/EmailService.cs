using Domain.OtherModels.EmailService;
using Domain.Shared.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Application.Features.Service.Administrator
{

    public static class EmailService
    {
        public static async Task SendQueuedEmail<T>(QueuedEmail<T> email)
        {
            await SendEmail(email.ToEmail, email.EmailSetting, email.BodyFormatter, email.Data, email.Flag);
        }

        public static async Task<bool> SendEmail<T>(string toEmail, EmailSettingObject emailSetting, Func<T, string> bodyFormatter, T data, string flag = "")
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailSetting.DisplayName, emailSetting.EmailAddress));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = emailSetting.Subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailSetting.IsBodyHtml ? bodyFormatter(data) : null,
                    TextBody = !emailSetting.IsBodyHtml ? bodyFormatter(data) : null
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                int port = int.Parse(emailSetting.Port);
                var secureOption = emailSetting.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;

                await client.ConnectAsync(emailSetting.Host, port, secureOption);

                if (!emailSetting.UseDefaultCredentials)
                {
                    await client.AuthenticateAsync(emailSetting.EmailAddress, emailSetting.EmailPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[EmailService] Error: {ex.Message}");
                return false;
            }
        }
    }

}
