using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OtherModels.Response
{
    public class PagedResponse<T>
    {  
        public IEnumerable<T> ListOfObject { get; set; }
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
