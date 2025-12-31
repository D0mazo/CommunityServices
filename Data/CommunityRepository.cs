using System;
using System.Collections.Generic;
using CommunityServices.Domain;
using Npgsql;

namespace CommunityServices.Data
{
    public class CommunityRepository : ICommunityRepository
    {
        public int Create(string name)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO communities (name)
                VALUES (@n)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("n", name);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(int id, string name)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                UPDATE communities
                SET name = @n
                WHERE id = @id;
            ", conn);

            cmd.Parameters.AddWithValue("n", name);
            cmd.Parameters.AddWithValue("id", id);

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Bendrija neegzistuoja.");
        }

        public void Delete(int id)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                DELETE FROM communities
                WHERE id = @id;
            ", conn);

            cmd.Parameters.AddWithValue("id", id);

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Bendrija neegzistuoja.");
        }

        public List<Community> GetAll()
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                SELECT id, name
                FROM communities
                ORDER BY name;
            ", conn);

            using var r = cmd.ExecuteReader();
            var list = new List<Community>();

            var idOrd = r.GetOrdinal("id");
            var nameOrd = r.GetOrdinal("name");

            while (r.Read())
                list.Add(new Community(r.GetInt32(idOrd), r.GetString(nameOrd)));

            return list;
        }
    }
}
