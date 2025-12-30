using CommunityServices.Domain;

namespace CommunityServices.Domain
{
    // Lengvas DTO admin vartotojų sąrašui
    public class UserListItem
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public Role Role { get; set; }
        public int? CommunityId { get; set; }
    }
}