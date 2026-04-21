using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.DTO
{
    public class CreateRoleResultDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? Description { get; set; }      
        public string? AgencyID { get; set; }      
    }
}
