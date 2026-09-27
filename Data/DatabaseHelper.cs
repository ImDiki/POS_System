using System;
using System.Data.SqlClient;
using System.IO;

namespace OICPOSレジ_2C29KS.Data
{
    public static class DatabaseHelper
    {
        private static readonly string DatabasePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "POSDATABASE.mdf");

        private static readonly string ConnectionString =
            $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={DatabasePath};Integrated Security=True;Connect Timeout=30";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
