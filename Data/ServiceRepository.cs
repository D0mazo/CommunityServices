using CommunityServices.Domain;
using System.Collections.Generic;
using System;


namespace CommunityServices.Data
{
    public class ServiceRepository : IServiceRepository
    {
        public int Create(string name, string? description)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO services (name, description) VALUES (@n, @d);
            SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@d", (object?)description ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(int id, string name, string? description)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE services SET name=@n, description=@d WHERE id=@id;";
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@d", (object?)description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Paslauga neegzistuoja.");
        }

        public void Delete(int id)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM services WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Paslauga neegzistuoja.");
        }

        public List<ServiceItem> GetAll()
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name, description FROM services ORDER BY name;";
            using var r = cmd.ExecuteReader();

            var list = new List<ServiceItem>();
            while (r.Read())
                list.Add(new ServiceItem(r.GetInt32("id"), r.GetString("name"),
                    r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description")));
            return list;
        }
    }
}
