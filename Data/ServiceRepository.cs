using CommunityServices.Domain;
using System.Collections.Generic;
using System;
using Npgsql;

namespace CommunityServices.Data
{
    public class ServiceRepository : IServiceRepository
    {
        public int Create(string name, string? description)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO services (name, description)
                VALUES (@n, @d)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("n", name);
            cmd.Parameters.AddWithValue("d", (object?)description ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(int id, string name, string? description)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                UPDATE services
                SET name = @n, description = @d
                WHERE id = @id;
            ", conn);

            cmd.Parameters.AddWithValue("n", name);
            cmd.Parameters.AddWithValue("d", (object?)description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("id", id);

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Paslauga neegzistuoja.");
        }

        public void Delete(int id)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                DELETE FROM services
                WHERE id = @id;
            ", conn);

            cmd.Parameters.AddWithValue("id", id);

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Paslauga neegzistuoja.");
        }

        public List<ServiceItem> GetAll()
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                SELECT id, name, description
                FROM services
                ORDER BY name;
            ", conn);

            using var r = cmd.ExecuteReader();
            var list = new List<ServiceItem>();

            var idOrd = r.GetOrdinal("id");
            var nameOrd = r.GetOrdinal("name");
            var descOrd = r.GetOrdinal("description");

            while (r.Read())
            {
                list.Add(new ServiceItem(
                    r.GetInt32(idOrd),
                    r.GetString(nameOrd),
                    r.IsDBNull(descOrd) ? null : r.GetString(descOrd)
                ));
            }

            return list;
        }
    }
}
