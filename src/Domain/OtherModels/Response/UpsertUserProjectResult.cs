using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OtherModels.Response
{
    public class UpsertUserProjectResult
    {
        public long ProjectId { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
