using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.OTP
{
    public class OTPVerification
    {
        [StringLength(200)]
        public string? Email { get; set; }      
        public DateTime? OTPLifeTime { get; set; }
        public DateTime? VerifiedTime { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
