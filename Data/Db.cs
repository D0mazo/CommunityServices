using Npgsql;
using System.Configuration;

namespace CommunityServices.Data
{
    public static class Db
    {
        public static NpgsqlConnection OpenConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["MyDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ConfigurationErrorsException("Connection string 'MyDb' nerastas App.config faile.");

            var conn = new NpgsqlConnection(cs);
            conn.Open();
            return conn;
        }
    }
}
