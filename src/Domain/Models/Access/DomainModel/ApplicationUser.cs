using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Access.DomainModel
{
    public class ApplicationUser : IdentityUser<Guid>
    {     
        [StringLength(50)]
        public string? EmployeeId { get; set; }
        [StringLength(100)]
        public string? EmployeeName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRoleActive { get; set; }
        [StringLength(50)]
        public string? RoleId { get; set; }
        [StringLength(150)]
        public string? Address { get; set; }
        public long? OrganizationId { get; set; }
        public long? CompanyId { get; set; }
        public long? DivisionId { get; set; }
        public bool? IsDefaultPassword { get; set; }
        public int PasswordChangedCount { get; set; }
        public string? Password { get; set; }
        [StringLength(100)]
        public string? DefaultCode { get; set; }
        public string? DefaultPasswordHash { get; set; }
        public string? DefaultSecurityStamp { get; set; }
        [Column(TypeName = "date")]
        public DateTime? PasswordExpiredDate { get; set; }
        public long? BranchId { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? AgencyID { get; set; }
        public string? RegistrationType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
