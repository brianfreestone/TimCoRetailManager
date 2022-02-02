using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData(ISqlDataAccess sql)
        {
            this._sql = sql;
        }
        public UserModel GetUserById(string Id)
        {
            UserModel user = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "TRMData").First();

            return user;
        }
    }
}
