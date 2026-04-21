using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Permission.DomainModel
{
    public class UserPermission
    {
        public string? PermissionID { get; set; }
        public bool IsAllowed { get; set; }
        public bool IsActive { get; set; }
        public string? ActivationStatus { get; set; }
        public bool IsApproved { get; set; }
        public string? StateStatus { get; set; }
    }
}
