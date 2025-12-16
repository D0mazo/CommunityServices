using System.Collections.Generic;

namespace CommunityServices.Data

{
    public class CommunityServiceRepository : ICommunityServiceRepository
    {
        public int AssignService(int communityId, int serviceId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO community_services (community_id, service_id)
VALUES (@c, @s);
SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@c", communityId);
            cmd.Parameters.AddWithValue("@s", serviceId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UnassignService(int communityId, int serviceId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
DELETE FROM community_services
WHERE community_id=@c AND service_id=@s;";
            cmd.Parameters.AddWithValue("@c", communityId);
            cmd.Parameters.AddWithValue("@s", serviceId);
        }

        public List<(int CommunityServiceId, string ServiceName, string? Description, decimal? Price, string Currency)>
            GetServicesForCommunity(int communityId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT cs.id AS cs_id,
       s.name AS service_name,
       s.description,
       p.price,
       COALESCE(p.currency,'EUR') AS currency
FROM community_services cs
JOIN services s ON s.id = cs.service_id
LEFT JOIN prices p ON p.community_service_id = cs.id
WHERE cs.community_id = @c
ORDER BY s.name;";
            cmd.Parameters.AddWithValue("@c", communityId);

            using var r = cmd.ExecuteReader();
            var list = new List<(int, string, string?, decimal?, string)>();
            while (r.Read())
            {
                decimal? price = r.IsDBNull(r.GetOrdinal("price")) ? null : r.GetDecimal("price");
                string? desc = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description");
                list.Add((r.GetInt32("cs_id"), r.GetString("service_name"), desc, price, r.GetString("currency")));
            }
            return list;
        }
    }
}
