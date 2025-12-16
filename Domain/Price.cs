namespace CommunityServices.Domain
{
    public class Price
    {
        private int _id;
        private int _communityServiceId;
        private decimal _value;
        private string _currency = "EUR";

        public int Id { get => _id; set => _id = value; }
        public int CommunityServiceId { get => _communityServiceId; set => _communityServiceId = value; }
        public decimal Value { get => _value; set => _value = value; }
        public string Currency { get => _currency; set => _currency = value; }

        public Price() { }

        public Price(int id, int communityServiceId, decimal value, string currency)
        {
            Id = id;
            CommunityServiceId = communityServiceId;
            Value = value;
            Currency = currency;
        }
    }
}
