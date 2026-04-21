using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.DTO
{
    public class GetApplicationRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        //public bool? IsSysadmin { get; set; }
        //public bool? IsGroupAdmin { get; set; }
        //public bool? IsCompanyAdmin { get; set; }
        //public bool? IsBranchAdmin { get; set; }
        public long? OrganizationId { get; set; }
        public long? CompanyId { get; set; }
    }
}
