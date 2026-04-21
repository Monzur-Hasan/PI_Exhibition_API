using Domain.OtherModels.EmailService;

namespace Domain.Shared.Configurations
{
    public class QueuedEmail<T>
    {
        public required string ToEmail { get; set; }
        public required EmailSettingObject EmailSetting { get; set; }
        public required Func<T, string> BodyFormatter { get; set; }
        public required T Data { get; set; }
        public required string Flag { get; set; }
    }

}
