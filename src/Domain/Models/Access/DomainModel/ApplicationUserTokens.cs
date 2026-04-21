using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models.Access.DomainModel
{
    public class ApplicationUserTokens<TKey> : IdentityUserToken<TKey> where TKey : IEquatable<TKey>
    {
        [StringLength(50)]
        public virtual string SessionId { get; set; }
        [StringLength(50)]
        public virtual string Status { get; set; } // Login, LogOut
        [StringLength(50)]
        public virtual string PublicIP { get; set; }
        [StringLength(50)]
        public virtual string PrivateIP { get; set; }
        [StringLength(100)]
        public virtual string DeviceModel { get; set; }
        [StringLength(100)]
        public virtual string DeviceType { get; set; }
        [StringLength(100)]
        public virtual string OS { get; set; }
        [StringLength(100)]
        public virtual string OSVersion { get; set; }
        [StringLength(100)]
        public virtual string Browser { get; set; }
        [StringLength(100)]
        public virtual string BrowserVersion { get; set; }
        [StringLength(100)]
        public string MACID { get; set; }
        public virtual DateTime? LoggedInTime { get; set; }
        public virtual DateTime? LoggedOutTime { get; set; }

    }
}
