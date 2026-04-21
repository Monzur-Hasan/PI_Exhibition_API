using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OtherModels.EmailService
{
    public class EmailSettingObject
    {
        [StringLength(300)]
        public string EmailAddress { get; set; }
        [StringLength(100)]
        public string EmailPassword { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; }
        [StringLength(100)]
        public string EmailFor { get; set; } // OTP
        public bool IsBodyHtml { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Port { get; set; }
        [StringLength(100)]
        public string Host { get; set; }
        [StringLength(500)]
        public string Subject { get; set; }
        public string EmailHtmlBody { get; set; }
        public string EmailTextBody { get; set; }
    }
}
