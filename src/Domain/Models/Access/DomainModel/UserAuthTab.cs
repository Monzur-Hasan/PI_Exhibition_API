using Domain.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Access.DomainModel
{
    public class UserAuthTab : BaseModel
    {
        public UserAuthTab()
        {
        }

        [Key]
        public long UATId { get; set; }
        public long? TaskId { get; set; }
        public long SubmenuId { get; set; }
        public long TabId { get; set; }
        [StringLength(100)]
        public string TabName { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Detail { get; set; }
        public bool Delete { get; set; }
        public bool Approval { get; set; }
        public bool Check { get; set; }
        public bool Accept { get; set; }
        public bool Report { get; set; }
        public bool Upload { get; set; }
        public string UserId { get; set; }
        public long CompanyId { get; set; }
        public long OrganizationId { get; set; }
        public long BranchId { get; set; }
    }
}
