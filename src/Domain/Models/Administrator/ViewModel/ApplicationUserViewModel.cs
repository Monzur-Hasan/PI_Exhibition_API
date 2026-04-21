using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.ViewModel
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        [Required, StringLength(150)]
        public string FullName { get; set; }
        //Required,
        [StringLength(50)]
        public string EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsRoleActive { get; set; }
        [Required, StringLength(50)]
        public string RoleId { get; set; }
        [StringLength(500)]
        public string AccessToken { get; set; }
        [StringLength(500)]
        public string IdToken { get; set; }
        [StringLength(300)]
        public string ProfilePicPath { get; set; }
        public byte[] ProfilePic { get; set; }
        [StringLength(50)]
        public string CurrentState { get; set; } // Login, LogOut
        [StringLength(150)]
        public string Address { get; set; }
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long? DivisionId { get; set; }
        public string DivisionName { get; set; }
        public long? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public long? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [StringLength(100)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        // 
        [Required, StringLength(256)]
        public string UserName { get; set; }
        [DataType(DataType.PhoneNumber), StringLength(50)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress), Required, StringLength(256)]
        public string Email { get; set; }
        //[Required,DataType(DataType.Password),StringLength(20)]
        //[DataType(DataType.Password), StringLength(20)]
        public string Password { get; set; }
        //[Required, DataType(DataType.Password), StringLength(20), Compare("Password")]
        //[DataType(DataType.Password), StringLength(20), Compare("Password")]
        public string ConfirmPassword { get; set; }
        public bool? IsDefaultPassword { get; set; }
        public bool? IsPasswordExpired { get; set; }
        public short RemainExpiredDate { get; set; }
        [StringLength(100)]
        public string DefaultCode { get; set; }

        public string? AgencyID { get; set; }
        public string RoleName { get; set; }
    }
}
