using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.OTP
{
    public class OTPRequestsViewModel
    {
        public long? RequestId { get; set; }
        [StringLength(50)]
        public string? RequestUniqId { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        [StringLength(50)]
        public string? PublicIP { get; set; }
        [StringLength(50)]
        public string? PrivateIP { get; set; }
        [StringLength(100)]
        public string? DeviceType { get; set; }
        [StringLength(100)]
        public string? OS { get; set; }
        [StringLength(100)]
        public string? OSVersion { get; set; }
        [StringLength(100)]
        public string? Browser { get; set; }
        [StringLength(100)]
        public string? BrowserVersion { get; set; }
        public int OTP { get; set; }
    }
}
