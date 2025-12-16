using CommunityServices.Data;
using CommunityServices.Domain;

namespace CommunityServices.Services
{
    public class AuthService
    {
        private readonly IUserRepository _users;

        public AuthService(IUserRepository users)
        {
            _users = users;
        }

        public User Login(string username, string password)
        {
            var row = _users.GetByUsername(username);
            if (row == null)
                throw new UnauthorizedAccessException("Neteisingas prisijungimo vardas arba slaptažodis.");

            if (!PasswordHasher.Verify(password, row.Value.passHash))
                throw new UnauthorizedAccessException("Neteisingas prisijungimo vardas arba slaptažodis.");

            return row.Value.role switch
            {
                Role.ADMIN => new AdminUser(row.Value.id, row.Value.username, row.Value.firstName, row.Value.lastName),
                Role.MANAGER => new ManagerUser(row.Value.id, row.Value.username, row.Value.firstName, row.Value.lastName),
                Role.RESIDENT => row.Value.communityId is int cid
                    ? new ResidentUser(row.Value.id, row.Value.username, row.Value.firstName, row.Value.lastName, cid)
                    : throw new InvalidOperationException("Gyventojas neturi priskirtos bendrijos."),
                _ => throw new InvalidOperationException("Nežinoma rolė.")
            };
        }
    }
}
