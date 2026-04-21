using Domain.ViewModels.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DapperObject
{
    public interface IClientDatabase
    {
        void ClietDbs();
        string GetDatabaseName(long organizationId);
        ClientDB GetClientObj(long organizationId);
        string GetDatabaseName(string username);
        ClientDB GetClientObj(string username);
    }
}
