using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.Filter
{
    public class ActivePermissions_Filter
    {
        public string? PermissionID { get; set; }
        public string? RoleID { get; set; }
        public string? RolePermissionID { get; set; } 
        public string? IsAllowed { get; set; }
        public string? AgencyID { get; set; }
        public string? MainMenuID { get; set; }
        public string? SubMenuID { get; set; }
        public string? PermissionKey { get; set; }
        public string? PermissionType { get; set; }
        public string? PermissionName { get; set; }
        public string? IsActive { get; set; }
        public string? IsApproved { get; set; }
        public string? StateStatus { get; set; }
        public string? ActivationStatus { get; set; }
    }
}
