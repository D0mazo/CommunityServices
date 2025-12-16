using CommunityServices.Data;
using CommunityServices.Domain;

namespace CommunityServices.Services
{
    public static class Seeder
    {
        public static void EnsureAdminExists(IUserRepository users)
        {
            var existing = users.GetByUsername("admin");
            if (existing != null) return;

            // default: admin/Admin123
            var hash = PasswordHasher.Hash("Admin123");
            users.CreateUser("admin", hash, "Admin", "Admin", Role.ADMIN, null);
        }
    }
}

