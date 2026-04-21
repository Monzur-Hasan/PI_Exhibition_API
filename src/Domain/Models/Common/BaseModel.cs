using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Common
{
    public class BaseModel
    {
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        public string? ActivationStatus { get; set; }
        public string? StateStatus { get; set; }
        [StringLength(100)]
        public string? CreatedBy { get; set; } = string.Empty;
        public Nullable<DateTime> CreatedDate { get; set; }
        [StringLength(100)]
        public string? UpdatedBy { get; set; } = string.Empty;
        public Nullable<DateTime> UpdatedDate { get; set; }
    }
}
