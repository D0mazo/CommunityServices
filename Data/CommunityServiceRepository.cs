using System;
using System.Collections.Generic;
using Npgsql;

namespace CommunityServices.Data
{
    public class CommunityServiceRepository : ICommunityServiceRepository
    {
        public int AssignService(int communityId, int serviceId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO community_services (community_id, service_id)
                VALUES (@c, @s)
                RETURNING id;
            ", conn);

            cmd.Parameters.AddWithValue("c", communityId);
            cmd.Parameters.AddWithValue("s", serviceId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UnassignService(int communityId, int serviceId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                DELETE FROM community_services
                WHERE community_id = @c AND service_id = @s;
            ", conn);

            cmd.Parameters.AddWithValue("c", communityId);
            cmd.Parameters.AddWithValue("s", serviceId);

            cmd.ExecuteNonQuery(); // <-- you were missing this
        }

        public List<(int CommunityServiceId, string ServiceName, string? Description, decimal? Price, string Currency)>
            GetServicesForCommunity(int communityId)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                SELECT cs.id AS cs_id,
                       s.name AS service_name,
                       s.description,
                       p.price,
                       COALESCE(p.currency,'EUR') AS currency
                FROM community_services cs
                JOIN services s ON s.id = cs.service_id
                LEFT JOIN prices p ON p.community_service_id = cs.id
                WHERE cs.community_id = @c
                ORDER BY s.name;
            ", conn);

            cmd.Parameters.AddWithValue("c", communityId);

            using var r = cmd.ExecuteReader();
            var list = new List<(int, string, string?, decimal?, string)>();

            var csIdOrd = r.GetOrdinal("cs_id");
            var serviceNameOrd = r.GetOrdinal("service_name");
            var descOrd = r.GetOrdinal("description");
            var priceOrd = r.GetOrdinal("price");
            var currencyOrd = r.GetOrdinal("currency");

            while (r.Read())
            {
                string? desc = r.IsDBNull(descOrd) ? null : r.GetString(descOrd);
                decimal? price = r.IsDBNull(priceOrd) ? null : r.GetDecimal(priceOrd);

                list.Add((
                    r.GetInt32(csIdOrd),
                    r.GetString(serviceNameOrd),
                    desc,
                    price,
                    r.GetString(currencyOrd)
                ));
            }

            return list;
        }
    }
}
