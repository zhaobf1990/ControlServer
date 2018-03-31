using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace Utility
{
    public class ConnectStringHelper
    {
        public static SqlConnection GetConnection(string serverAddress, string dbname, string username, string pwd)
        {
            string str = ConfigurationManager.ConnectionStrings["NetwaySDKCSSample.Properties.Settings.ConnectionString"].ToString();

            str = str.Replace("{Data Source}", serverAddress)
                .Replace("{Initial Catalog}", dbname)
                .Replace("{User ID}", username)
                .Replace("{Password}", pwd);

            return new SqlConnection(str);
        }
    }
}
