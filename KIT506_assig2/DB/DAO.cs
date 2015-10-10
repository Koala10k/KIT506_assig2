using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.DB
{
    abstract class DAO
    {
        private const string db = "kit206";
        private const string user = "kit206";
        private const string pwd = "kit206";
        private const string ds = "alacritas.cis.utas.edu.au";

        private static MySqlConnection conn = null;

        public static MySqlConnection GetConnection()
        {
            if (conn == null)
            {
                //Note: This approach is not thread-safe
                string connectionString = String.Format("Database={0};Data Source={1};User Id={2};Password={3}", db, ds, user, pwd);
                conn = new MySqlConnection(connectionString);
            }
            return conn;
        }
    }
}
