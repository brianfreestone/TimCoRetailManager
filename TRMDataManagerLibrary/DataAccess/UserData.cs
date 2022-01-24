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
    public class UserData
    {
        private readonly IConfiguration _config;

        public UserData(IConfiguration config)
        {
            this._config = config;
        }
        public UserModel GetUserById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var p = new { Id = Id };

            UserModel user = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData").First();

            return user;
        }
    }
}
