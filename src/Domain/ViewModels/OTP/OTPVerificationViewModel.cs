using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.OTP
{
    public class OTPVerificationViewModel
    {
        [StringLength(200)]
        public string? Email { get; set; }
        [StringLength(6)]
        public string? OTP { get; set; }    
        public string? Token { get; set; }
        public DateTime? OTPLifeTime { get; set; }
        public DateTime? VerifiedTime { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
