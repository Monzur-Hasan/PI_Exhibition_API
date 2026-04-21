using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Head_Model.DomainModel
{
    public class Head
    {
        public int ID { get; set; }
        public int HeadTypeID { get; set; }
        public bool? IsActive { get; set; }     
        public string? HeadName { get; set; }
        public string? Description { get; set; }
        public string? UserID { get; set; }      
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
