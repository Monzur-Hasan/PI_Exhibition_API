using Domain.Models.Access.DomainModel;
using Domain.Shared.Configurations;

namespace Application.Features.Service.Email_Setting
{
    public interface IEmailQueueService
    {
        void Enqueue<T>(QueuedEmail<T> email);
        bool TryDequeue(out object email);
        Task WaitForNewItemAsync(CancellationToken cancellationToken);
        Task SendAsync(string to, string subject, string htmlBody);
        Task SendWelcomeEmailAsync(ApplicationUser user, string plainPassword);
    }

}
