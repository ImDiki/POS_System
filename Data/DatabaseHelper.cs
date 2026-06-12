using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace OICPOSレジ_2C29KS.Data
{
   public class DatabaseHelper
    {
        private static readonly string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\OIC_2年\PG\第５\OICPOSレジ_2C29KS\OICPOSレジ_2C29KS\POSDATABASE.mdf;Integrated Security=True";

      
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
