using CommunityServices.Domain;
using System.Collections.Generic;



namespace CommunityServices.Domain
{
    // Vartotojas (bendras) - paveldėjimo bazė
    public abstract class User
    {
        private int _id;
        private string _username = "";
        private string _firstName = "";
        private string _lastName = "";
        private Role _role;
        private int? _communityId;

        public int Id { get => _id; protected set => _id = value; }
        public string Username { get => _username; protected set => _username = value; }
        public string FirstName { get => _firstName; protected set => _firstName = value; }
        public string LastName { get => _lastName; protected set => _lastName = value; }
        public Role Role { get => _role; protected set => _role = value; }
        public int? CommunityId { get => _communityId; protected set => _communityId = value; }

        protected User(int id, string username, string firstName, string lastName, Role role, int? communityId)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            CommunityId = communityId;
        }

        // Polimorfizmas: tos pačios funkcijos - skirtingas elgesys rolėms
        public abstract IReadOnlyList<string> GetAllowedActions();
    }

    public sealed class AdminUser : User
    {
        public AdminUser(int id, string username, string firstName, string lastName)
            : base(id, username, firstName, lastName, Role.ADMIN, null) { }

        public override IReadOnlyList<string> GetAllowedActions() => new[]
        {
            "ManageCommunities",
            "ManageServices",
            "ManageUsers",
            "AutoGenerateCredentials"
        };
    }

    public sealed class ManagerUser : User
    {
        public ManagerUser(int id, string username, string firstName, string lastName)
            : base(id, username, firstName, lastName, Role.MANAGER, null) { }

        public override IReadOnlyList<string> GetAllowedActions() => new[]
        {
            "AssignServiceToCommunity",
            "SetServicePrices"
        };
    }

    public sealed class ResidentUser : User
    {
        public ResidentUser(int id, string username, string firstName, string lastName, int communityId)
            : base(id, username, firstName, lastName, Role.RESIDENT, communityId) { }

        public override IReadOnlyList<string> GetAllowedActions() => new[]
        {
            "ViewMyCommunityServicesAndPrices",
            "SearchService"
        };
    }
}
