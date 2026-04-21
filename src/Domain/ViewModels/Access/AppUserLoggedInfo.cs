using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.Access
{
    public class AppUserLoggedInfo
    {

        public string AgencyID { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public long DivisionId { get; set; }
        public string DivisionName { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public long GradeId { get; set; }
        public string GradeName { get; set; }
        public long DesignationId { get; set; }
        public string DesignationName { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long SectionId { get; set; }
        public string SectionName { get; set; }
        public long SubSectionId { get; set; }
        public string SubSectionName { get; set; }
        public long CompanyId { get; set; }
        public long OrganizationId { get; set; }
        public long WorkShiftId { get; set; }
        public string WorkShiftName { get; set; }
        public bool? IsDefaultPassword { get; set; }
        public string DefaultCode { get; set; }
        public string PasswordExpiredDate { get; set; }
        public short RemainExpireDays { get; set; }
        public bool IsActive { get; set; }
        public string SiteThumbnailPath { get; set; }
        public string SiteShortName { get; set; }
        public string PhotoPath { get; set; }
        public string CompanyName { get; set; }
        public string OrganizationName { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}
