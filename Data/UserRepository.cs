using CommunityServices.Domain;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace CommunityServices.Data
{
    public class UserRepository : IUserRepository
    {
        public (int id, string username, string passHash, string firstName, string lastName, Role role, int? communityId)?
            GetByUsername(string username)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT id, username, password_hash, first_name, last_name, role, community_id
            FROM users
            WHERE username = @u
            LIMIT 1;";
            cmd.Parameters.AddWithValue("@u", username);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var role = Enum.Parse<Role>(r.GetString("role"));

            int? communityId =
                r.IsDBNull(r.GetOrdinal("community_id"))
                ? null
                : r.GetInt32("community_id");

            return (
                r.GetInt32("id"),
                r.GetString("username"),
                r.GetString("password_hash"),
                r.GetString("first_name"),
                r.GetString("last_name"),
                role,
                communityId
            );
        }

        public int CreateUser(string username, string passwordHash, string firstName, string lastName, Role role, int? communityId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            INSERT INTO users (username, password_hash, first_name, last_name, role, community_id)
            VALUES (@u, @p, @fn, @ln, @r, @cid);
            SELECT LAST_INSERT_ID();";

            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", passwordHash);
            cmd.Parameters.AddWithValue("@fn", firstName);
            cmd.Parameters.AddWithValue("@ln", lastName);
            cmd.Parameters.AddWithValue("@r", role.ToString());
            cmd.Parameters.AddWithValue("@cid", (object?)communityId ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void DeleteUser(int userId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"DELETE FROM users WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", userId);

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Vartotojas neegzistuoja.");
        }
    }
}
