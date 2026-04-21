using Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.DomainModel
{
    public class RolePermission: BaseModel
    {      
        public long ID { get; set; }

        public Guid RoleID { get; set; }

        public string? PermissionID { get; set; }
        public string? AgencyID { get; set; }

        public bool? IsAllowed { get; set; }
    }
}
