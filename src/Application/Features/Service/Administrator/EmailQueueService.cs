using Application.Features.Service.Email_Setting;
using Domain.Models.Access.DomainModel;
using Domain.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
namespace Application.Features.Service.Administrator
{

    // Infrastructure/Services/EmailQueueService.cs
    public class EmailQueueService : IEmailQueueService
    {
        private readonly ConcurrentQueue<object> _emailQueue = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly IConfiguration _configuration;
        public EmailQueueService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Enqueue<T>(QueuedEmail<T> email)
        {
            _emailQueue.Enqueue(email);
            _signal.Release();
        }

        public bool TryDequeue(out object email)
        {
            return _emailQueue.TryDequeue(out email);
        }

        public async Task WaitForNewItemAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            // Example: push to queue / smtp / background job
            // keep simple here
            await Task.Run(() =>
            {
                // enqueue email
            });
        }

        public async Task SendWelcomeEmailAsync(ApplicationUser user, string plainPassword)
        {
            var loginUrl = _configuration["AppSettings:ClientOrigin"];

            var html = EmailTemplate.WelcomeEmailTemplate(
                user.EmployeeName ?? user.UserName,
                user.UserName,
                plainPassword,
                loginUrl
            );

            await SendAsync(
                user.Email,
                "Welcome to NGIT Services - Login Information",
                html
            );
        }
    }
}
