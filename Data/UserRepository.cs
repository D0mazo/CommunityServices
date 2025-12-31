using CommunityServices.Domain;
using Npgsql;
using NpgsqlTypes;
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
            using var cmd = new NpgsqlCommand(@"
                SELECT id, username, password_hash, first_name, last_name, role::text AS role, community_id
                FROM users
                WHERE username = @u
                LIMIT 1;
            ", conn);

            cmd.Parameters.AddWithValue("u", username);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var id64 = r.GetInt64(r.GetOrdinal("id"));
            if (id64 > int.MaxValue) throw new OverflowException("users.id viršija int ribas.");
            var id = (int)id64;

            var roleStr = r.GetString(r.GetOrdinal("role"));
            var role = Enum.Parse<Role>(roleStr, ignoreCase: false);

            int? communityId = null;
            var cidOrd = r.GetOrdinal("community_id");
            if (!r.IsDBNull(cidOrd))
            {
                var cid64 = r.GetInt64(cidOrd);
                if (cid64 > int.MaxValue) throw new OverflowException("users.community_id viršija int ribas.");
                communityId = (int)cid64;
            }

            return (
                id,
                r.GetString(r.GetOrdinal("username")),
                r.GetString(r.GetOrdinal("password_hash")),
                r.GetString(r.GetOrdinal("first_name")),
                r.GetString(r.GetOrdinal("last_name")),
                role,
                communityId
            );
        }

        public int CreateUser(string username, string passwordHash, string firstName, string lastName, Role role, int? communityId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO users (username, password_hash, first_name, last_name, role, community_id)
                VALUES (@u, @p, @fn, @ln, @r::user_role, @cid)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("p", passwordHash);
            cmd.Parameters.AddWithValue("fn", firstName);
            cmd.Parameters.AddWithValue("ln", lastName);

            // siunčiam tekstą, SQL daro cast į ENUM
            cmd.Parameters.Add("r", NpgsqlDbType.Text).Value = role.ToString();

            // svarbu: tipinis BIGINT parametras, kad nebūtų 42P08 kai null
            cmd.Parameters.Add("cid", NpgsqlDbType.Bigint).Value = (object?)communityId ?? DBNull.Value;

            var idObj = cmd.ExecuteScalar();
            var id64 = Convert.ToInt64(idObj);
            if (id64 > int.MaxValue) throw new OverflowException("users.id viršija int ribas.");
            return (int)id64;
        }

        public void DeleteUser(int userId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                DELETE FROM users
                WHERE id = @id;
            ", conn);

            // DB'e id BIGINT, bet kontraktas int - nurodom tipą aiškiai
            cmd.Parameters.Add("id", NpgsqlDbType.Bigint).Value = userId;

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Vartotojas neegzistuoja.");
        }

        public List<UserListItem> GetAll(string? firstNameFilter)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                SELECT id, username, first_name, last_name, role::text AS role, community_id
                FROM users
                WHERE (@fn::text IS NULL OR first_name ILIKE '%' || @fn::text || '%')
                ORDER BY id DESC;
            ", conn);

            cmd.Parameters.AddWithValue("fn",
                string.IsNullOrWhiteSpace(firstNameFilter) ? (object)DBNull.Value : firstNameFilter.Trim());

            using var r = cmd.ExecuteReader();
            var list = new List<UserListItem>();

            while (r.Read())
            {
                var id64 = r.GetInt64(r.GetOrdinal("id"));
                if (id64 > int.MaxValue) throw new OverflowException("users.id viršija int ribas.");
                var id = (int)id64;

                var roleStr = r.GetString(r.GetOrdinal("role"));
                var role = Enum.Parse<Role>(roleStr, ignoreCase: false);

                int? communityId = null;
                var cidOrd = r.GetOrdinal("community_id");
                if (!r.IsDBNull(cidOrd))
                {
                    var cid64 = r.GetInt64(cidOrd);
                    if (cid64 > int.MaxValue) throw new OverflowException("users.community_id viršija int ribas.");
                    communityId = (int)cid64;
                }

                list.Add(new UserListItem
                {
                    Id = id,
                    Username = r.GetString(r.GetOrdinal("username")),
                    FirstName = r.GetString(r.GetOrdinal("first_name")),
                    LastName = r.GetString(r.GetOrdinal("last_name")),
                    Role = role,
                    CommunityId = communityId
                });
            }

            return list;
        }

        public void UpdateUser(int id, string username, string firstName, string lastName, Role role, int? communityId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                UPDATE users
                SET username = @u,
                    first_name = @fn,
                    last_name = @ln,
                    role = @r::user_role,
                    community_id = @cid
                WHERE id = @id;
            ", conn);

            // id BIGINT DB'e
            cmd.Parameters.Add("id", NpgsqlDbType.Bigint).Value = id;

            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("fn", firstName);
            cmd.Parameters.AddWithValue("ln", lastName);

            // role tekstas + ::user_role
            cmd.Parameters.Add("r", NpgsqlDbType.Text).Value = role.ToString();

            // tipinis BIGINT (kad nebūtų 42P08)
            cmd.Parameters.Add("cid", NpgsqlDbType.Bigint).Value = (object?)communityId ?? DBNull.Value;

            if (cmd.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Vartotojas neegzistuoja.");
        }
    }
}
