using Domain.OtherModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Access.DTO
{
    public class SubMenuFilter : Sortparam
    {
        public string? SubmenuName { get; set; }
    }
}
