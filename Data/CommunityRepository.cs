using System;
using CommunityServices.Domain;
using System.Collections.Generic;


namespace CommunityServices.Data
{
    public class CommunityRepository : ICommunityRepository
    {
        public int Create(string name)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO communities (name) VALUES (@n);
SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@n", name);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(int id, string name)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE communities SET name=@n WHERE id=@id;";
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@id", id);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Bendrija neegzistuoja.");
        }

        public void Delete(int id)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM communities WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Bendrija neegzistuoja.");
        }

        public List<Community> GetAll()
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name FROM communities ORDER BY name;";
            using var r = cmd.ExecuteReader();

            var list = new List<Community>();
            while (r.Read())
                list.Add(new Community(r.GetInt32("id"), r.GetString("name")));
            return list;
        }
    }
}
