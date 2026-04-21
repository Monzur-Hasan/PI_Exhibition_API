using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.Access
{
    public class ClientDB
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientStatus { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string OrgCode { get; set; }
    }
}
