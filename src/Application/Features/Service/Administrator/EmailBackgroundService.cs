using System.Net.Mail;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application.Features.Service.Email_Setting;
using Domain.Shared.Configurations;

namespace Application.Features.Service.Administrator
{

    // Infrastructure/Services/EmailBackgroundService.cs
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailQueueService _queueService;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(IEmailQueueService queueService, ILogger<EmailBackgroundService> logger)
        {
            _queueService = queueService;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"BG service is running");
                await _queueService.WaitForNewItemAsync(stoppingToken);

                if (_queueService.TryDequeue(out var item))
                {
                    try
                    {
                        _logger.LogInformation("Sending email to {To}", item);
                        var emailType = item.GetType();
                        var method = typeof(EmailBackgroundService).GetMethod(nameof(SendEmailAsync), BindingFlags.NonPublic | BindingFlags.Instance);
                        var generic = method.MakeGenericMethod(emailType.GenericTypeArguments[0]);
                        await (Task)generic.Invoke(this, new[] { item });
                        _logger.LogInformation("Email sent to {To}", item);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send email");
                        throw new Exception($"Error sending email: {ex.Message}", ex);
                    }
                }
            }
        }

        private async Task SendEmailAsync<T>(QueuedEmail<T> email)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(email.EmailSetting.EmailAddress, email.EmailSetting.DisplayName),
                    Subject = email.EmailSetting.Subject,
                    Body = email.BodyFormatter(email.Data),
                    IsBodyHtml = email.EmailSetting.IsBodyHtml
                };

                message.To.Add(email.ToEmail);

                using var smtp = new SmtpClient(email.EmailSetting.Host, Convert.ToInt32(email.EmailSetting.Port))
                {
                    EnableSsl = email.EmailSetting.EnableSsl,
                    UseDefaultCredentials = email.EmailSetting.UseDefaultCredentials,
                    Credentials = new NetworkCredential(email.EmailSetting.EmailAddress, email.EmailSetting.EmailPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending email: {ex.Message}", ex);
            }
        }
    }
}
