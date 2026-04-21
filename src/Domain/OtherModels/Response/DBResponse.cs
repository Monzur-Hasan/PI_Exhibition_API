using Domain.OtherModels.Pagination;
using System.Collections.Generic;

namespace Domain.OtherModels.Response
{
    public class DBResponse<T> where T : class
    {
        public T Object { get; set; } // SQL Single Row
        public IEnumerable<T> ListOfObject { get; set; } // SQL List
        public Pageparam Pageparam { get; set; } // SQL Page Params
    }


    public class DBResponse : Pageparam
    {
        public string JSONData { get; set; }   
     
    }
}
