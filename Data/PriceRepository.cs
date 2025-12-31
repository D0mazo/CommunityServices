using Npgsql;

namespace CommunityServices.Data
{
    public class PriceRepository : IPriceRepository
    {
        public void UpsertPrice(int communityServiceId, decimal price, string currency)
        {
            using var conn = Db.OpenConnection();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO prices (community_service_id, price, currency)
                VALUES (@cs, @p, @cur)
                ON CONFLICT (community_service_id)
                DO UPDATE SET price = EXCLUDED.price,
                              currency = EXCLUDED.currency,
                              updated_at = now();
            ", conn);

            cmd.Parameters.AddWithValue("cs", communityServiceId);
            cmd.Parameters.AddWithValue("p", price);
            cmd.Parameters.AddWithValue("cur", currency);

            cmd.ExecuteNonQuery();
        }
    }
}
