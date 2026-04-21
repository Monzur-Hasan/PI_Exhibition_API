using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.DomainModel
{
    public class SubMenu
    {
        public string SubmenuId { get; set; }
        public string SubmenuName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Path { get; set; }
        public string Component { get; set; }
        public string IconClass { get; set; }
        public string IconColor { get; set; }
        public bool IsViewable { get; set; }
        public bool IsActAsParent { get; set; }
        public bool HasTab { get; set; }
        public string ParentSubmenuId { get; set; }
        public bool IsActive { get; set; }
        public string MMId { get; set; }
        public long ModuleId { get; set; }
        public long ApplicationId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int MenuSequence { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

}
