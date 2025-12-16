namespace CommunityServices.Data
{
    public class PriceRepository : IPriceRepository
    {
        public void UpsertPrice(int communityServiceId, decimal price, string currency)
        {
            using var conn = Db.OpenConnection();
            using var cmd = conn.CreateCommand();
            // MySQL upsert
            cmd.CommandText = @"
INSERT INTO prices (community_service_id, price, currency)
VALUES (@cs, @p, @cur)
ON DUPLICATE KEY UPDATE price=@p, currency=@cur;";
            cmd.Parameters.AddWithValue("@cs", communityServiceId);
            cmd.Parameters.AddWithValue("@p", price);
            cmd.Parameters.AddWithValue("@cur", currency);
            cmd.ExecuteNonQuery();
        }
    }
}
