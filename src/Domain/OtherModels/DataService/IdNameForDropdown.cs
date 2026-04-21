using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OtherModels.DataService
{
    public class IdNameForDropdown
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public string? Name { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Qty { get; set; }
        public string? NameDesc { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
}
