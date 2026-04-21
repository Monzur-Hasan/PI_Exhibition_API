using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Access.DTO
{
    public class MainMenuInputDto
    {
        public string MenuName { get; set; }
        public string ShortName { get; set; }
        public string IconClass { get; set; }
        public string IconColor { get; set; }
        public long MId { get; set; }
        public string CreatedBy { get; set; }
        public long ApplicationId { get; set; }
        public bool IsActive { get; set; }
        public int SequenceNo { get; set; }
        public string ServiceID { get; set; }
    }
}
