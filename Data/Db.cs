using MySql.Data.MySqlClient;
using MySqlConnector;
using System.Configuration;
using System.Collections.Generic;


namespace CommunityServices.Data
{
    public static class Db
    {
        public static MySqlConnection OpenConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            var conn = new MySqlConnection(cs);
            conn.Open();
            return conn;
        }
    }
}
