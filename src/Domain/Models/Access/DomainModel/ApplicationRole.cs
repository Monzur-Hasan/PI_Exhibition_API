using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models.Access.DomainModel
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public bool? IsActive { get; set; }
        [StringLength(100)]
        public string? Description { get; set; }
        public string? RegistrationType { get; set; }
        public bool? IsSysadmin { get; set; }
        public bool? IsGroupAdmin { get; set; }
        public bool? IsCompanyAdmin { get; set; }
        public bool? IsBranchAdmin { get; set; }
        public long? CompanyId { get; set; }
        public long? OrganizationId { get; set; }
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
        public string? AgencyID { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}