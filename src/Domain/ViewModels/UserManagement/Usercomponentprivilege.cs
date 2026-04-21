using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.UserManagement
{
    public class Usercomponentprivilege
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Component { get; set; }
        [Range(1, long.MaxValue)]
        public long CompanyId { get; set; }
        [Range(1, long.MaxValue)]
        public long OrganizationId { get; set; }
    }
}
