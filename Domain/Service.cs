namespace CommunityServices.Domain
{
    public class ServiceItem
    {
        private int _id;
        private string _name = "";
        private string? _description;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string? Description { get => _description; set => _description = value; }

        public ServiceItem() { }
        public ServiceItem(int id, string name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
