using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.DbClient
{
    public class DbClientFactory
    {
        public static IDbClient Create()
        {
            IDbClient dbHelper = null;
            try
            {
                
                switch (McConfig.Instance.DbType)
                {
                    case "MsSql":
                        dbHelper = new MsSqlClient();
                        break;
                    case "Oracle":
                        dbHelper = new OracleClient();
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                
            }
            return dbHelper;
        }
    }
}
